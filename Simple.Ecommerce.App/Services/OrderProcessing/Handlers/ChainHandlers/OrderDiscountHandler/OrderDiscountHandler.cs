using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.OrderDiscountHandler
{
    public class OrderDiscountHandler : BaseOrderProcessingHandler
    {
        public OrderDiscountHandler() : base() { }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var orderDiscount = orderInProcess.UnAppliedDiscounts.FirstOrDefault(op => op.DiscountScope == DiscountScope.Order);
                if (orderDiscount is not null)
                {
                    decimal amountDiscounted = 0;
                    OrderDiscountAppliedEvent publishEvent = null!;
                    switch (orderDiscount.DiscountType)
                    {
                        case DiscountType.FirstPurchase:
                            if (orderDiscount.DiscountValueType == DiscountValueType.Percentage)
                            {
                                amountDiscounted = orderInProcess.CurrentTotalPrice * orderDiscount.Value!.Value;
                            }
                            else if (orderDiscount.DiscountValueType == DiscountValueType.FixedAmount)
                            {
                                amountDiscounted = orderDiscount.Value!.Value;
                            }
                            break;
                        case DiscountType.Percentage:
                            amountDiscounted = orderInProcess.CurrentTotalPrice * orderDiscount.Value!.Value;
                            break;
                        case DiscountType.FixedAmount:
                            amountDiscounted = orderDiscount.Value!.Value;
                            break;
                        case DiscountType.FreeShipping:
                            amountDiscounted = 0;
                            break;
                    }
                    if (!(amountDiscounted > orderInProcess.CurrentTotalPrice))
                    {
                        publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, amountDiscounted);
                        Console.WriteLine($"\t[OrderDiscountHandler] Desconto {publishEvent.DiscountName} aplicado ao pedido. Valor descontado do pedido: {publishEvent.AmountDiscounted:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                    }
                    else
                    {
                        Console.WriteLine($"\t[OrderDiscountHandler] Desconto {publishEvent.DiscountName} não foi aplicado ao pedido por resultar em preço negativo. Valor atual do pedido: {orderInProcess.CurrentTotalPrice:C}. Valor que seria descontado: {publishEvent.AmountDiscounted:C}.");
                    }

                    orderInProcess.RemoveAppliedDiscount(orderDiscount);
                }
            }

            await base.Handle(orderInProcess);
        }
    }
}
