using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBundleHandler
{
    public class BundleItemsDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;

        public BundleItemsDiscountHandler(
            IDiscountBundleItemRepository discountBundleItemRepository
        ) : base()
        {
            _discountBundleItemRepository = discountBundleItemRepository;
        }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var bundleDiscounts = orderInProcess.UnAppliedDiscounts
                    .Where(uad => uad.DiscountType == DiscountType.Bundle && uad.DiscountScope == DiscountScope.Product)
                    .ToList();
                int index = bundleDiscounts.Count - 1;

                while (index >= 0)
                {
                    var discount = bundleDiscounts[index];
                    var sameBundleDiscounts = bundleDiscounts.Where(bd => bd.Id == discount.Id).ToList();

                    var getBundleItems = await _discountBundleItemRepository.GetByDiscountId(discount.Id);
                    if (getBundleItems.IsFailure)
                    {
                        throw new ResultException(new Error("BundleItemDiscountsHandler.NotFound", $"Os itens do pacote para o desconto {discount.Name} não foram encontrados!"));
                    }

                    var bundleItems = getBundleItems.GetValue();
                    var productIds = bundleItems.Select(bi => bi.ProductId).ToList();
                    var orderBundleItems = orderInProcess.Items.Where(i => productIds.Contains(i.ProductId)).ToList();

                    if (orderBundleItems.Count == bundleItems.Count)
                    {
                        var bundleItemsInformation = (
                            from orderItem in orderBundleItems
                            join bundleItem in bundleItems
                            on orderItem.ProductId equals bundleItem.ProductId
                            select new OrderItemWithBundleItem
                            {
                                OrderItem = orderItem,
                                BundleItem = bundleItem
                            }).ToList();

                        var bundleCount = int.MaxValue;
                        foreach (var information in bundleItemsInformation)
                        {
                            int possibleBundles = information.OrderItem.Quantity / information.BundleItem.Quantity;
                            if (possibleBundles < bundleCount)
                                bundleCount = possibleBundles;
                            if (bundleCount <= 0)
                                break;
                        }
                        if (bundleCount > 0)
                        {
                            var bundleDetails = new List<BundleItemDetail>();
                            foreach (var information in bundleItemsInformation)
                            {
                                decimal amountDiscountedPrice = 0;
                                if (discount.DiscountValueType == DiscountValueType.Percentage)
                                    amountDiscountedPrice = information.OrderItem.CurrentPrice * discount.Value!.Value;
                                else
                                    amountDiscountedPrice = discount.Value!.Value;
                                bundleDetails.Add(new BundleItemDetail(
                                    information.OrderItem.ProductId,
                                    information.BundleItem.Quantity,
                                    amountDiscountedPrice
                                ));
                            }

                            for (var i = 0; i < bundleCount; i++)
                            {
                                var publishEvent = orderInProcess.ApplyBundleItemDiscount(bundleDetails, discount.Id, discount.Name, discount.DiscountType);
                                Console.WriteLine($"\t[BundleItemDiscountsHandler] Desconto {discount.Name} aplicado ao pedido. Total descontado do pedido: {publishEvent.AmountDiscountedTotal:C}. Novo total do pedido: {publishEvent.CurrentTotal:C}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\t[BundleItemDiscountsHandler] Desconto {discount.Name} não foi aplicado aos items do pacote por não ter quantidade suficiente de cada item do pacote.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\t[BundleItemDiscountsHandler] Desconto {discount.Name} não foi aplicado aos items do pacote por não ter todos os items do pacote.");
                    }

                    foreach (var bd in sameBundleDiscounts)
                    {
                        orderInProcess.RemoveAppliedDiscount(bd);
                        bundleDiscounts.Remove(bd);
                    }
                    index = bundleDiscounts.Count - 1;
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }

        private class OrderItemWithBundleItem
        {
            public OrderItemInProcess OrderItem { get; set; }
            public DiscountBundleItem BundleItem { get; set; }
        }
    }
}
