using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBOGOHandler
{
    public class RevertBOGOItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(BOGODiscountAppliedEvent);
        private readonly IRevertOrderUnitOfWork _revertOrderUoW;

        public RevertBOGOItemDiscountHandler(
            IRevertOrderUnitOfWork revertOrderUoW
        ) : base()
        {
            _revertOrderUoW = revertOrderUoW;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var orderItemId = eventData["OriginalOrderItemId"].AsInt32;
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            int? couponId = eventData.Contains("CouponId") && !eventData["CouponId"].IsBsonNull
                ? eventData["CouponId"].AsInt32
                : null;
            string? couponCode = eventData.Contains("CouponCode") && !eventData["CouponCode"].IsBsonNull
                ? eventData["CouponCode"].AsString
                : null;
            var amountDiscounted = eventData["AmountDiscounted"].AsDecimal;

            var item = order.Items.FirstOrDefault(item => item.Id == orderItemId);
            if (item is null)
            {
                Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.");
                return Result<bool>.Failure(new List<Error> { new("RevertBOGOItemsDiscountHandler.NotFound", $"O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.") });
            }

            var getDiscount = await _revertOrderUoW.Discounts.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertBOGOItemsDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            Coupon? coupon = null;
            if (couponId is not null)
            {
                var getCoupon = await _revertOrderUoW.Coupons.Get(couponId.Value);
                if (getCoupon.IsFailure)
                {
                    Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] Os dados do cupom '{couponCode}' aplicado ao produto {item.ProductName} não foram encontrados.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBOGOItemsDiscountHandler.NotFound", $"Os dados do cupom '{couponCode}' aplicado ao produto {item.ProductName} não foram encontrados.") });
                }
                coupon = getCoupon.GetValue();
                coupon.SetUsed(false);
                if (coupon.Validate() is { IsFailure: true } result)
                {
                    Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBOGOItemsDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.") });
                }
                var updateResult = await _revertOrderUoW.Coupons.Update(coupon);
                if (updateResult.IsFailure)
                {
                    Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBOGOItemsDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao produto {item.ProductName}.") });
                }
            }

            var publishEvent = order.RevertBOGOItemDiscount(orderItemId, discountId, discountName, discountType, couponId, couponCode, amountDiscounted);
            Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] O desconto {discountName} foi removido do produto {item.ProductName}. Valor Revertido: {publishEvent.AmountReverted:C}. Novo Total: {publishEvent.CurrentTotal:C}.");

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
