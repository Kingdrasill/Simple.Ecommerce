using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class OrderInProcess
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public OrderType OrderType { get; private set; }
        public Address Address { get; private set; }
        public PaymentInformation? PaymentInformation { get; private set; }
        public OrderLock OrderLock { get; private set; }
        public decimal OriginalTotalPrice { get; private set; }
        public decimal CurrentTotalPrice { get; private set; }
        public List<OrderItemInProcess> Items { get; private set; }
        public List<AppliedDiscountItem> FreeItems { get; private set; }
        public List<AppliedBundle> Bundles { get; private set; }
        public List<DiscountInProcess> UnAppliedDiscounts { get; private set; }
        public AppliedDiscountDetail? AppliedDiscount { get; private set; }
        public decimal TotalDiscount { get; private set; }
        public decimal ShippingFee { get; private set; }
        public decimal TaxAmount { get; private set; }
        public string Status { get; private set; }
        private bool shippingFeeApplied { get; set; }

        private readonly List<IOrderProcessingEvent> _events = new List<IOrderProcessingEvent>();

        public OrderInProcess(int id, int userId, OrderType orderType, Address address, PaymentInformation? paymentInformation, OrderLock orderLock, decimal originalTotalPrice, List<OrderItemInProcess> items, List<DiscountInProcess> unAppliedDiscounts, string initialStatus)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            PaymentInformation = paymentInformation;
            OrderLock = orderLock;
            OriginalTotalPrice = originalTotalPrice;
            CurrentTotalPrice = originalTotalPrice;
            Items = items;
            FreeItems = new();
            Bundles = new();
            UnAppliedDiscounts = unAppliedDiscounts;
            AppliedDiscount = null;
            TotalDiscount = 0;
            ShippingFee = 0;
            TaxAmount = 0;
            Status = initialStatus;
            shippingFeeApplied = false;
        }

        public OrderInProcess(int id, int userId, OrderType orderType, Address address, PaymentInformation? paymentInformation, OrderLock orderLock, decimal totalPrice, List<OrderItemInProcess> items, List<AppliedDiscountItem> freeItems, List<AppliedBundle> bundles, AppliedDiscountDetail? appliedDiscount, decimal totalDiscount, decimal shippingFee, decimal taxAmount, string finalStatus)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            PaymentInformation = paymentInformation;
            OrderLock = orderLock;
            OriginalTotalPrice = totalPrice;
            CurrentTotalPrice = totalPrice;
            Items = items;
            FreeItems = freeItems;
            Bundles = bundles;
            UnAppliedDiscounts = new();
            AppliedDiscount = appliedDiscount;
            TotalDiscount = totalDiscount;
            ShippingFee = shippingFee;
            TaxAmount = taxAmount;
            Status = finalStatus;
            shippingFeeApplied = true;
        }

        public OrderDiscountAppliedEvent ApplyOrderDiscount(int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountDiscounted)
        {
            if (AppliedDiscount is not null)
            {
                throw new InvalidOperationException("Um pedido não pode ter mais de um desconto aplicado a ele!");
            }

            decimal amountDiscountedTotal = 0;
            if (discountType == DiscountType.FreeShipping)
            {
                if (!shippingFeeApplied)
                {
                    throw new Exception("Desconto de entrega grátis não pode ser aplicado antes de calcular o custo de entrega!");
                }
                amountDiscountedTotal = ShippingFee;
            }
            else
            {
                amountDiscountedTotal = amountDiscounted;
            }

            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            AppliedDiscount = new AppliedDiscountDetail(discountId, discountName, discountType, couponId, couponCode);

            var publishEvent = new OrderDiscountAppliedEvent(Id, discountId, discountName, discountType, couponId, couponCode, CurrentTotalPrice, amountDiscountedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public OrderDiscountRevertedEvent RevertOrderDiscount(int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountReverted)
        {
            TotalDiscount -= amountReverted;
            CurrentTotalPrice += amountReverted;
            AppliedDiscount = null;
            var publishEvent = new OrderDiscountRevertedEvent(Id, discountId, discountName, discountType, couponId, couponCode, CurrentTotalPrice, amountReverted);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public SimpleItemDiscountAppliedEvent ApplySimpleItemDiscount(int productId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountDiscountedPrice)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            decimal itemOriginalTotal = item.GetTotal();
            item.ApplyDiscount(discountId, discountName, discountType, couponId, couponCode, amountDiscountedPrice);
            decimal amountDiscountedTotal = itemOriginalTotal - item.GetTotal();

            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var publishEvent = new SimpleItemDiscountAppliedEvent(Id, item.Id, discountId, discountName, discountType, couponId, couponCode, item.CurrentPrice, CurrentTotalPrice, amountDiscountedPrice, amountDiscountedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public SimpleItemDiscountRevertEvent RevertSimpleItemDiscount(int orderItemId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountRevertedPrice, decimal amountRevertedTotal)
        {
            var item = Items.First(i => i.Id == orderItemId);
            item.RevertDiscount(amountRevertedPrice);

            TotalDiscount -= amountRevertedTotal;
            CurrentTotalPrice += amountRevertedTotal;

            var publishEvent = new SimpleItemDiscountRevertEvent(Id, orderItemId, discountId, discountName, discountType, couponId, couponCode, item.CurrentPrice, CurrentTotalPrice, amountRevertedPrice, amountRevertedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public TieredItemDiscountAppliedEvent ApplyTieredItemDiscount(int productId, int discountId, string discountName, DiscountType discountType, int tierId, string tierName, int? couponId, string? couponCode, decimal amountDiscountedPrice)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            decimal itemOriginalTotal = item.GetTotal();
            item.ApplyDiscount(discountId, discountName, discountType, couponId, couponCode, amountDiscountedPrice);
            decimal amountDiscountedTotal = itemOriginalTotal - item.GetTotal();

            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var publishEvent = new TieredItemDiscountAppliedEvent(Id, item.Id, discountId, discountName, discountType, tierId, tierName, couponId, couponCode, item.CurrentPrice, CurrentTotalPrice, amountDiscountedPrice, amountDiscountedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public TieredItemDiscountRevertEvent RevertTieredItemDiscount(int orderItemId, int discountId, string discountname, DiscountType discountType, int tierId, string tierName, int? couponId, string? couponCode, decimal amountRevertedPrice, decimal amountRevertedTotal)
        {
            var item = Items.First(i => i.Id == orderItemId);
            item.RevertDiscount(amountRevertedPrice);

            TotalDiscount -= amountRevertedTotal;
            CurrentTotalPrice += amountRevertedTotal;

            var publishEvent = new TieredItemDiscountRevertEvent(Id, orderItemId, discountId, discountname, discountType, tierId, tierName, couponId, couponCode, item.CurrentPrice, CurrentTotalPrice, amountRevertedPrice, amountRevertedTotal);
            AddEvent(publishEvent); 
            return publishEvent;
        }

        public BOGODiscountAppliedEvent ApplyBOGOItemDiscount(int productId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            decimal amountDiscounted = item.UnitPrice;
            var freedItem = item.RemoveNItems(1);
            
            var freeItem = new AppliedDiscountItem(
                freedItem.Id, 
                freedItem.ProductId, 
                freedItem.ProductName, 
                1, 
                0, 
                amountDiscounted, 
                new AppliedDiscountDetail(
                    discountId, 
                    discountName, 
                    discountType, 
                    couponId,
                    couponCode
                ));            
            FreeItems.Add(freeItem);
            TotalDiscount += amountDiscounted;
            CurrentTotalPrice -= amountDiscounted;

            var publishEvent = new BOGODiscountAppliedEvent(Id, freedItem.Id, freedItem.ProductId, freedItem.ProductName, discountId, discountName, discountType, couponId, couponCode, CurrentTotalPrice, amountDiscounted);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public BOGODiscountRevertEvent RevertBOGOItemDiscount(int orderItemId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountReverted)
        {
            var item = Items.First(item => item.Id == orderItemId);
            var freeItem = FreeItems.First(item => item.OrderItemId == orderItemId);

            item.AddNItems(1);
            FreeItems.Remove(freeItem);

            TotalDiscount -= amountReverted;
            CurrentTotalPrice += amountReverted;

            var publishEvent = new BOGODiscountRevertEvent(Id, orderItemId, item.ProductId, item.ProductName, discountId, discountName, discountType, couponId, couponCode, CurrentTotalPrice, amountReverted);
            AddEvent(publishEvent); 
            return publishEvent;
        }

        public BundleDiscountAppliedEvent ApplyBundleItemDiscount(List<BundleItemDetail> bundleItemsDetail, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode)
        {
            var bundle = new List<AppliedDiscountItem>();
            var amountDiscountedTotal = 0m;
            foreach (var bundleItemDetail in bundleItemsDetail)
            {
                var item = Items.First(i => i.ProductId == bundleItemDetail.ProductId);
                if (item.AppliedDiscount is not null)
                {
                    throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
                }

                var bundledItems = item.RemoveNItems(bundleItemDetail.Quantity);
                var bundledItemsOriginalTotal = bundledItems.GetTotal();
                bundledItems.ApplyDiscount(discountId, discountName, discountType, couponId, couponCode, bundleItemDetail.AmountDiscountedPrice);
                var discountTotal = bundledItemsOriginalTotal - bundledItems.GetTotal();
                amountDiscountedTotal += discountTotal;

                bundle.Add(new AppliedDiscountItem(
                    bundledItems.Id,
                    bundledItems.ProductId,
                    bundledItems.ProductName,
                    bundledItems.Quantity,
                    bundledItems.CurrentPrice,
                    bundleItemDetail.AmountDiscountedPrice,
                    new AppliedDiscountDetail(
                        discountId, 
                        discountName, 
                        discountType, 
                        couponId,
                        couponCode
                    )));
            }
            var appliedBundle = new AppliedBundle(discountId, discountName, discountType, couponId, couponCode, bundle);
            Bundles.Add(appliedBundle);
            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var bundleItemEntries = bundle.Select(b => new BundleItemAppliedEntry(
                b.OrderItemId,
                b.ProductId,
                b.ProductName,
                b.Quantity,
                b.AmountDiscountedPrice,
                b.CurrentPrice
            )).ToList();
            var publishEvent = new BundleDiscountAppliedEvent(Id, discountId, discountName, discountType, couponId, couponCode, appliedBundle.Id, CurrentTotalPrice, bundleItemEntries, amountDiscountedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public BundleDiscountRevertEvent RevertBundleItemDiscount(Guid bundleId, List<RevertBundleItemDetail> bundleItemsDetails, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal amountRevertedTotal)
        {
            var bundle = Bundles.First(b => b.Id == bundleId);
            List<BundleItemRevertEntry> bundleItemRevertEntries = new(); 
            foreach (var bundleItemDetail in bundleItemsDetails)
            {
                var item = Items.First(i => i.Id == bundleItemDetail.OrderItemId);
                var bItem = bundle.Items.First(b => b.OrderItemId == bundleItemDetail.OrderItemId);

                item.AddNItems(bItem.Quantity);
                bundle.Items.Remove(bItem);

                bundleItemRevertEntries.Add(new BundleItemRevertEntry(
                    bundleItemDetail.OrderItemId,
                    bundleItemDetail.ProductId,
                    item.ProductName,
                    bundleItemDetail.Quantity,
                    bundleItemDetail.AmountRevertedPrice,
                    item.CurrentPrice
                ));
            }
            Bundles.Remove(bundle);
            TotalDiscount -= amountRevertedTotal;
            CurrentTotalPrice += amountRevertedTotal;

            var publishEvent = new BundleDiscountRevertEvent(Id, discountId, discountName, discountType, couponId, couponCode, bundleId, CurrentTotalPrice, bundleItemRevertEntries, amountRevertedTotal);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public void ApplyShippingFee(decimal fee)
        {
            ShippingFee = fee;
            CurrentTotalPrice += fee;
            shippingFeeApplied = true;
            AddEvent(new ShippingFeeAppliedEvent(Id, fee, CurrentTotalPrice));
        }

        public void RevertShippingFee(decimal fee)
        {
            ShippingFee -= fee;
            CurrentTotalPrice -= fee;
            shippingFeeApplied = false;
            AddEvent(new ShippingFeeRevertedEvent(Id, fee, CurrentTotalPrice));
        }

        public void ApplyTaxes(decimal tax)
        {
            TaxAmount = tax;
            CurrentTotalPrice += tax;
            AddEvent(new TaxAppliedEvent(Id, tax, CurrentTotalPrice));
        }

        public void RevertTaxes(decimal tax)
        {
            TaxAmount -= tax;
            CurrentTotalPrice -= tax;
            AddEvent(new TaxRevertedEvent(Id, tax, CurrentTotalPrice));
        }

        public void AddUnappliedDiscount(DiscountInProcess orderDiscountInProcess)
        {
            UnAppliedDiscounts.Add(orderDiscountInProcess);
        }

        public void RemoveAppliedDiscount(DiscountInProcess orderDiscountInProcess)
        {
            UnAppliedDiscounts.Remove(orderDiscountInProcess);
        }

        public void AddEvent(IOrderProcessingEvent @event) 
        {
            _events.Add(@event);
        }

        public IReadOnlyCollection<IOrderProcessingEvent> GetUncommittedEvents()
        {
            return _events.AsReadOnly();
        }

        public void ClearUncommittedEvents()
        {
            _events.Clear();
        }
    }
}
