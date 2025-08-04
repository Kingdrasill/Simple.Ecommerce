using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBOGOHandler
{
    public class BOGOItemsDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;

        public BOGOItemsDiscountHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW
        ) : base() 
        {
            _confirmOrderUoW = confirmOrderUoW;
        }

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
                        int? couponId = discount.Coupon is null ? null : discount.Coupon.Id;
                        string? couponCode = discount.Coupon is null ? null : discount.Coupon.Code;
                        var publishEvent = orderInProcess.ApplyBOGOItemDiscount(item.ProductId, discount.Id, discount.Name, discount.DiscountType, couponId, couponCode);
                        Console.WriteLine($"\t[BOGOItemsDiscountsHandler] O desconto de {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Total descontado do pedido: {publishEvent.AmountDiscounted:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                        if (discount.Coupon is not null)
                        {
                            var getCoupon = await _confirmOrderUoW.Coupons.Get(discount.Coupon.Id);
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
