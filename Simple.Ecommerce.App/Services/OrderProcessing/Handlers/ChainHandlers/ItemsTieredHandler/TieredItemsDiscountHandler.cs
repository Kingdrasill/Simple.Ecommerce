using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsTieredHandler
{
    public class TieredItemsDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;

        public TieredItemsDiscountHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW
        ) : base()
        {
            _confirmOrderUoW = confirmOrderUoW;
        }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var tieredDiscounts = orderInProcess.UnAppliedDiscounts
                    .Where(uad => uad.DiscountType == DiscountType.Tiered && uad.DiscountScope == DiscountScope.Product)
                    .ToList();
                int index = tieredDiscounts.Count - 1;

                while (index >= 0)
                {
                    var discount = tieredDiscounts[index];
                    var item = orderInProcess.Items.First(i => i.Id == discount.OwnerId);
                    decimal amountDiscountedPrice = 0;

                    var getItemTiers = await _confirmOrderUoW.DiscountTiers.GetByDiscountId(discount.Id);
                    if (getItemTiers.IsFailure)
                    {
                        throw new ResultException(new Error("TieredItemDiscountsHandler.NotFound", $"Os tiers para o desconto {discount.Name} do item {item.ProductName} não foram encontrados!"));
                    }

                    var tierToApply = getItemTiers.GetValue()
                        .Where(it => it.MinQuantity <= item.Quantity)
                        .OrderByDescending(it  => it.MinQuantity)
                        .FirstOrDefault();

                    if (tierToApply is not null)
                    {

                        if (discount.DiscountValueType == DiscountValueType.Percentage)
                        {
                            amountDiscountedPrice = item.CurrentPrice * tierToApply!.Value;
                        }
                        else if (discount.DiscountValueType == DiscountValueType.FixedAmount)
                        {
                            amountDiscountedPrice = discount.Value!.Value;
                        }
                        int? couponId = discount.Coupon is null ? null : discount.Coupon.Id;
                        string? couponCode = discount.Coupon is null ? null : discount.Coupon.Code;
                        var publishEvent = orderInProcess.ApplyTieredItemDiscount(item.ProductId, discount.Id, discount.Name, discount.DiscountType, tierToApply.Id, tierToApply.Name, couponId, couponCode, amountDiscountedPrice);
                        Console.WriteLine($"\t[TieredItemDiscountsHandler] O desconto {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Novo preço do item: {publishEvent.ItemPrice:C}. Total descontado do pedido: {publishEvent.AmountDiscountedTotal:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
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
                        Console.WriteLine($"\t[TieredItemDiscountsHandler] O desconto {discount.Name} não foi aplicado ao item {item.ProductName} por não ter a quantidade miníma do tier mais baixo.");
                    }

                    orderInProcess.RemoveAppliedDiscount(discount);
                    tieredDiscounts.RemoveAt(index);
                    index = tieredDiscounts.Count - 1;
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
