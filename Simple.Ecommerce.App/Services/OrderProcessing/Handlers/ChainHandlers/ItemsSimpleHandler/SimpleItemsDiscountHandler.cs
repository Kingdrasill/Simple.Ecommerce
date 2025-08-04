using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsSimpleHandler
{
    public class SimpleItemsDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly ICouponRepository _couponRepository;

        public SimpleItemsDiscountHandler(
            ICouponRepository couponRepository
        ) : base() 
        {
            _couponRepository = couponRepository;
        }

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
                        int? couponId = discount.Coupon is null ? null : discount.Coupon.Id;
                        string? couponCode = discount.Coupon is null ? null : discount.Coupon.Code;
                        var publishEvent = orderInProcess.ApplySimpleItemDiscount(item.ProductId, discount.Id, discount.Name, discount.DiscountType, couponId, couponCode, amountDiscountedPrice);
                        Console.WriteLine($"\t[SimpleItemDiscountsHandler] O desconto {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Novo preço do item: {publishEvent.ItemPrice:C}. Total descontado do pedido: {publishEvent.AmountDiscountedTotal:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                        if (discount.Coupon is not null)
                        {
                            var getCoupon = await _couponRepository.Get(discount.Coupon.Id);
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
                            var updateResult = await _couponRepository.Update(coupon);
                            if (updateResult.IsFailure)
                            {
                                throw new ResultException(updateResult.Errors!);
                            }
                        }
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
