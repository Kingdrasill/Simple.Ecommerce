using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBOGOHandler
{
    public class BOGOItemsDiscountHandler : BaseOrderProcessingHandler
    {
        public BOGOItemsDiscountHandler() : base() { }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var bogoDiscounts = orderInProcess.UnAppliedDiscounts
                    .Where(uad => uad.DiscountType == DiscountType.BuyOneGetOne && uad.DiscountScope == DiscountScope.Product)
                    .ToList();
                int index = bogoDiscounts.Count - 1;

                while (index >= 0)
                {
                    var discount = bogoDiscounts[index];
                    var item = orderInProcess.Items.First(i => i.Id == discount.OwnerId);

                    if (item.Quantity > 1)
                    {
                        var publishEvent = orderInProcess.ApplyBOGOItemDiscount(item.ProductId, discount.Id, discount.Name, discount.DiscountType);
                        Console.WriteLine($"\t[BOGOItemsDiscountsHandler] O desconto de {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Total descontado do pedido: {publishEvent.AmountDiscounted:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                    }
                    else
                    {
                        Console.WriteLine($"\t[BOGOItemsDiscountsHandler] O desconto {discount.Name} não foi aplicado ao item {item.ProductName} por não ter a quantidade necessária para o desconto.");
                    }

                    orderInProcess.RemoveAppliedDiscount(discount);
                    bogoDiscounts.RemoveAt(index);
                    index = bogoDiscounts.Count - 1;
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
