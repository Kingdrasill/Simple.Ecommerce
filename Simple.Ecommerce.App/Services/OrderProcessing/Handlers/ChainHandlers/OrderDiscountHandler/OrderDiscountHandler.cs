using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.OrderDiscountHandler
{
    public class OrderDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;

        public OrderDiscountHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW
        ) : base() 
        {
            _confirmOrderUoW = confirmOrderUoW;
        }

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
                        int? couponId = orderDiscount.Coupon is null ? null : orderDiscount.Coupon.Id;
                        string? couponCode = orderDiscount.Coupon is null ? null : orderDiscount.Coupon.Code;
                        publishEvent = orderInProcess.ApplyOrderDiscount(orderDiscount.Id, orderDiscount.Name, orderDiscount.DiscountType, couponId, couponCode, amountDiscounted);
                        Console.WriteLine($"\t[OrderDiscountHandler] Desconto {publishEvent.DiscountName} aplicado ao pedido. Valor descontado do pedido: {publishEvent.AmountDiscounted:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                        if (orderDiscount.Coupon is not null)
                        {
                            var getCoupon = await _confirmOrderUoW.Coupons.Get(orderDiscount.Coupon.Id);
                            if (getCoupon.IsFailure)
                            {
                                throw new ResultException(getCoupon.Errors!);
                            }
                            var coupon = getCoupon.GetValue();
                            coupon.SetUsed(true);
                            if (coupon.Validate() is { IsFailure: true } result)
                            {
                                throw new ResultException(result.Errors!);
                            }
                            var updateResult = await _confirmOrderUoW.Coupons.Update(coupon);
                            if (updateResult.IsFailure)
                            {
                                throw new ResultException(updateResult.Errors!);
                            }
                        }
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
