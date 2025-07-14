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
                            publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, amountDiscounted);

                            Console.WriteLine($"\t[OrderDiscountHandler] Desconto de primeira compra aplicado ao pedido. Valor descontado do pedido: {amountDiscounted}. Novo total do pedido: {orderInProcess.CurrentTotalPrice}");
                            break;
                        case DiscountType.Percentage:
                            amountDiscounted = orderInProcess.CurrentTotalPrice * orderDiscount.Value!.Value;
                            publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, amountDiscounted);

                            Console.WriteLine($"\t[OrderDiscountHandler] Desconto de porcentagem aplicado ao pedido. Valor descontado do pedido: {amountDiscounted}. Novo total do pedido: {orderInProcess.CurrentTotalPrice}");
                            break;
                        case DiscountType.FixedAmount:
                            amountDiscounted = orderDiscount.Value!.Value;
                            publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, amountDiscounted);

                            Console.WriteLine($"\t[OrderDiscountHandler] Desconto de preço fixo aplicado ao pedido. Valor descontado do pedido: {amountDiscounted}. Novo total do pedido: {orderInProcess.CurrentTotalPrice}");
                            break;
                        case DiscountType.FreeShipping:
                            publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, 0);

                            Console.WriteLine($"\t[OrderDiscountHandler] Desconto de frete grátis. Valor descontado do pedido: {orderInProcess.ShippingFee}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}");
                            break;
                    }
                    orderInProcess.RemoveAppliedDiscount(orderDiscount);
                }
            }

            await base.Handle(orderInProcess);
        }
    }
}
