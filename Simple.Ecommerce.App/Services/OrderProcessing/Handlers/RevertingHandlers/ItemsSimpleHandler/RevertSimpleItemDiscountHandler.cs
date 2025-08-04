using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsSimpleHandler
{
    public class RevertSimpleItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(SimpleItemDiscountAppliedEvent);
        private readonly IRevertOrderUnitOfWork _revertOrderUoW;

        public RevertSimpleItemDiscountHandler(
            IRevertOrderUnitOfWork revertOrderUoW
        ) : base()
        {
            _revertOrderUoW = revertOrderUoW;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var orderItemId = eventData["OrderItemId"].AsInt32;
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            int? couponId = eventData.Contains("CouponId") && !eventData["CouponId"].IsBsonNull
                ? eventData["CouponId"].AsInt32
                : null;
            string? couponCode = eventData.Contains("CouponCode") && !eventData["CouponCode"].IsBsonNull
                ? eventData["CouponCode"].AsString
                : null;
            var amountDiscountedPrice = eventData["AmountDiscountedPrice"].AsDecimal;
            var amountDiscountedTotal = eventData["AmountDiscountedTotal"].AsDecimal;

            var item = order.Items.FirstOrDefault(item => item.Id == orderItemId);
            if (item is null)
            {
                Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.");
                return Result<bool>.Failure(new List<Error>{ new("RevertSimpleItemsDiscountHandler.NotFound", $"O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.") });
            }

            var getDiscount = await _revertOrderUoW.Discounts.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertSimpleItemsDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            Coupon? coupon = null;
            if (couponId is not null)
            {
                var getCoupon = await _revertOrderUoW.Coupons.Get(couponId.Value);
                if (getCoupon.IsFailure)
                {
                    Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] Os dados do cupom '{couponCode}' aplicado ao produto {item.ProductName} não foram encontrados.");
                    return Result<bool>.Failure(new List<Error> { new("RevertSimpleItemsDiscountHandler.NotFound", $"Os dados do cupom '{couponCode}' aplicado ao produto {item.ProductName} não foram encontrados.") });
                }
                coupon = getCoupon.GetValue();
                coupon.SetUsed(false);
                if (coupon.Validate() is { IsFailure: true } result)
                {
                    Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.");
                    return Result<bool>.Failure(new List<Error> { new("RevertSimpleItemsDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.") });
                }
                var updateResult = await _revertOrderUoW.Coupons.Update(coupon);
                if (updateResult.IsFailure)
                {
                    Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.");
                    return Result<bool>.Failure(new List<Error> { new("RevertSimpleItemsDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.") });
                }
            }

            var publishEvent = order.RevertSimpleItemDiscount(orderItemId, discountId, discountName, discountType, couponId, couponCode, amountDiscountedPrice, amountDiscountedTotal);
            Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] O desconto {discountName} foi removido do produto {item.ProductName}. Valor Revertido: {publishEvent.AmountRevertedTotal:C}. Novo Total: {publishEvent.CurrentTotal:C}.");

            order.AddUnappliedDiscount(new DiscountInProcess(
                discountId,
                orderItemId,
                discountName,
                discountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive,
                coupon is null
                    ? null
                    : new CouponInProcess(
                        coupon.Id,
                        coupon.DiscountId,
                        coupon.Code,
                        coupon.ExpirationAt,
                        coupon.IsUsed
                    )));

            return Result<bool>.Success(true);
        }
    }
}
