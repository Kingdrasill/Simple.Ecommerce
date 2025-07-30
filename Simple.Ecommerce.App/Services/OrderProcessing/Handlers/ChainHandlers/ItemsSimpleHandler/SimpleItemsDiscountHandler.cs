using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsSimpleHandler
{
    public class SimpleItemsDiscountHandler : BaseOrderProcessingHandler
    {
        public SimpleItemsDiscountHandler() : base() { }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var simpleDiscounts = orderInProcess.UnAppliedDiscounts
                    .Where(uad => uad.DiscountType is DiscountType.Percentage or DiscountType.FixedAmount && uad.DiscountScope == DiscountScope.Product)
                    .ToList();
                int index = simpleDiscounts.Count - 1;

                while (index >= 0)
                {
                    var discount = simpleDiscounts[index];
                    var item = orderInProcess.Items.First(i => i.Id == discount.OwnerId);
                    decimal amountDiscountedPrice = 0;

                    if (discount.DiscountType == DiscountType.Percentage)
                    {
                        amountDiscountedPrice = item.CurrentPrice * discount.Value!.Value;
                    }
                    else if (discount.DiscountType == DiscountType.FixedAmount)
                    {
                        amountDiscountedPrice = discount.Value!.Value;
                    }
                    
                    if (!(amountDiscountedPrice > item.CurrentPrice))
                    {
                        var publishEvent = orderInProcess.ApplySimpleItemDiscount(item.ProductId, discount.Id, discount.Name, discount.DiscountType, amountDiscountedPrice);
                        Console.WriteLine($"\t[SimpleItemDiscountsHandler] O desconto {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Novo preço do item: {publishEvent.NewItemPrice:C}. Total descontado do pedido: {publishEvent.AmountDiscountedTotal:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                    }
                    else
                    {
                        Console.WriteLine($"\t[SimpleItemDiscountsHandler] O desconto {discount.Name} não foi aplicado ao item {item.ProductName} do pedido por resultar em um preço negativo. Preço do item: {item.CurrentPrice:C}. Valor que seria descontado do preço: {amountDiscountedPrice:C}.");
                    }

                    orderInProcess.RemoveAppliedDiscount(discount);
                    simpleDiscounts.RemoveAt(index);
                    index = simpleDiscounts.Count - 1;
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
