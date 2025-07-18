using Simple.Ecommerce.Domain.Enums.Discount;
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
        public decimal OrginalTotalPrice { get; private set; }
        public decimal CurrentTotalPrice { get; private set; }
        public List<OrderItemInProcess> Items { get; private set; }
        public List<AppliedDiscountItem> FreeItems { get; private set; }
        public List<AppliedBundle> Bundles { get; private set; }
        public List<OrderDiscountInProcess> UnAppliedDiscounts { get; private set; }
        public AppliedDiscountDetail? AppliedDiscount { get; private set; }
        public decimal TotalDiscount { get; private set; }
        public decimal ShippingFee { get; private set; }
        public decimal TaxAmount { get; private set; }
        public string Status { get; private set; }
        private bool shippingFeeApplied { get; set; }

        private readonly List<IOrderProcessingEvent> _events = new List<IOrderProcessingEvent>();

        public OrderInProcess(int id, int userId, OrderType orderType, Address address, PaymentInformation? paymentInformation, decimal originalTotalPrice, List<OrderItemInProcess> items, List<OrderDiscountInProcess> unAppliedDiscounts, string initialStatus)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            PaymentInformation = paymentInformation;
            OrginalTotalPrice = originalTotalPrice;
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

        public OrderDiscountAppliedEvent ApplyOrderDiscount(int discountId, string discountName, DiscountType discountType, decimal amountDiscounted)
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

            AppliedDiscount = new AppliedDiscountDetail(discountId, discountName, discountType);

            var publishEvent = new OrderDiscountAppliedEvent(Id, discountId, discountName, discountType, amountDiscountedTotal, CurrentTotalPrice);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public SimpleItemDiscountAppliedEvent ApplySimpleItemDiscount(int productId, int discountId, string discountName, DiscountType discountType, decimal amountDiscountedPrice)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            if (discountType == DiscountType.BuyOneGetOne)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemDiscount não trata de descontos do tipo compre 1 leve 1!", nameof(discountType));
            }
            else if (discountType == DiscountType.Bundle)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemDiscount não trata de descontos do tipo de pacote!", nameof(discountType));
            }

            decimal itemTotal = item.GetTotal();
            item.ApplyDiscount(discountId, discountName, discountType, amountDiscountedPrice);
            decimal amountDiscountedTotal = itemTotal - item.GetTotal();

            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var publishEvent = new SimpleItemDiscountAppliedEvent(Id, item.Id, discountId, discountName, discountType, amountDiscountedPrice, item.CurrentPrice, amountDiscountedTotal, CurrentTotalPrice);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public TieredItemDiscountAppliedEvent ApplyTieredItemDiscount(int productId, int discountId, string discountName, DiscountType discountType, string tierName, decimal amountDiscountedPrice)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            if (discountType == DiscountType.BuyOneGetOne)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemDiscount não trata de descontos do tipo compre 1 leve 1!", nameof(discountType));
            }
            else if (discountType == DiscountType.Bundle)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemDiscount não trata de descontos do tipo de pacote!", nameof(discountType));
            }

            decimal itemTotal = item.GetTotal();
            item.ApplyDiscount(discountId, discountName, discountType, amountDiscountedPrice);
            decimal amountDiscountedTotal = itemTotal - item.GetTotal();

            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var publishEvent = new TieredItemDiscountAppliedEvent(Id, item.Id, discountId, discountName, discountType, tierName, amountDiscountedPrice, item.CurrentPrice, amountDiscountedTotal, CurrentTotalPrice);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public BOGOItemDiscountAppliedEvent ApplyBOGOItemDiscount(int productId, int discountId, string discountName, DiscountType discountType)
        {
            var item = Items.First(i => i.ProductId == productId);
            if (item.AppliedDiscount is not null)
            {
                throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
            }

            if (discountType != DiscountType.BuyOneGetOne)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemBuyOneGetOneDiscount só trata de descontos do tipo de compre 1 leve 1!", nameof(discountType));
            }

            decimal amountDiscounted = item.UnitPrice;
            var freedItem = item.RemoveNItems(1);
            
            var freeItem = new AppliedDiscountItem(freedItem.Id, freedItem.ProductId, freedItem.ProductName, 1, 0, amountDiscounted, new AppliedDiscountDetail(discountId, discountName, discountType));            
            FreeItems.Add(freeItem);
            TotalDiscount += amountDiscounted;
            CurrentTotalPrice -= amountDiscounted;

            var publishEvent = new BOGOItemDiscountAppliedEvent(Id, freedItem.Id, freedItem.ProductId, freedItem.ProductName, discountId, discountName, discountType, amountDiscounted, CurrentTotalPrice);
            AddEvent(publishEvent);
            return publishEvent;
        }

        public BundleDiscountAppliedEvent ApplyBundleItemDiscount(List<BundleItemDetail> bundleItems, int discountId, string discountName, DiscountType discountType)
        {
            if (discountType != DiscountType.Bundle)
            {
                throw new ArgumentException($"O metódo ApplyOrderItemBundleDiscount só trata de descontos do tipo de pacote!", nameof(discountType));
            }

            var bundle = new List<AppliedDiscountItem>();
            var amountDiscountedTotal = 0m;
            foreach (var bundleItem in  bundleItems)
            {
                var item = Items.First(i => i.ProductId == bundleItem.ProductId);
                if (item.AppliedDiscount is not null)
                {
                    throw new InvalidOperationException($"O item {item.ProductName} pedido não pode ter mais de um desconto aplicado a ele!");
                }

                var bundledItem = item.RemoveNItems(bundleItem.Quantity);
                var bundledItemTotal = bundledItem.GetTotal();
                bundledItem.ApplyDiscount(discountId, discountName, discountType, bundleItem.AmountDiscountedPrice);
                var discountTotal = bundledItemTotal - bundledItem.GetTotal();
                amountDiscountedTotal += discountTotal;

                bundle.Add(new AppliedDiscountItem(
                    bundledItem.Id,
                    bundledItem.ProductId,
                    bundledItem.ProductName,
                    bundledItem.Quantity,
                    bundledItem.CurrentPrice,
                    bundleItem.AmountDiscountedPrice,
                    new AppliedDiscountDetail(discountId, discountName, discountType)
                ));
            }
            var appliedBundle = new AppliedBundle(discountId, discountName, discountType, bundle);
            Bundles.Add(appliedBundle);
            TotalDiscount += amountDiscountedTotal;
            CurrentTotalPrice -= amountDiscountedTotal;

            var bundleItemEntries = bundle.Select(b => new BundleItemEntry(
                b.OriginalOrderItemId,
                b.ProductId,
                b.ProductName,
                b.Quantity,
                b.CurrentPrice,
                b.AmountDiscountedPrice
            )).ToList();
            var publishEvent = new BundleDiscountAppliedEvent(Id, discountId, discountName, discountType, appliedBundle.Id, bundleItemEntries, amountDiscountedTotal, CurrentTotalPrice);
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

        public void ApplyTaxes(decimal tax)
        {
            TaxAmount = tax;
            CurrentTotalPrice += tax;
            AddEvent(new TaxAppliedEvent(Id, tax, CurrentTotalPrice));
        }

        public void RemoveAppliedDiscount(OrderDiscountInProcess orderDiscountInProcess)
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
