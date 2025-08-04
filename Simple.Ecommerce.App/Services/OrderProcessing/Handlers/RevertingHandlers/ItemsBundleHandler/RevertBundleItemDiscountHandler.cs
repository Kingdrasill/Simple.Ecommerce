using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBundleHandler
{
    public class RevertBundleItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(BundleDiscountAppliedEvent);
        private readonly IRevertOrderUnitOfWork _revertOrderUoW;

        public RevertBundleItemDiscountHandler(
            IRevertOrderUnitOfWork revertOrderUoW
        ) : base()
        {
            _revertOrderUoW = revertOrderUoW;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            int? couponId = eventData.Contains("CouponId") && !eventData["CouponId"].IsBsonNull
                ? eventData["CouponId"].AsInt32
                : null;
            string? couponCode = eventData.Contains("CouponCode") && !eventData["CouponCode"].IsBsonNull
                ? eventData["CouponCode"].AsString
                : null;
            var bundleId = eventData["BundleId"].AsGuid;
            var bsonArray = eventData["BundleItems"].AsBsonArray;
            var bundleItems = bsonArray
                .Select(b => BsonSerializer.Deserialize<BundleItemAppliedEntry>(b.AsBsonDocument))
                .ToList();
            var amountDiscountedTotal = eventData["AmountDiscountedTotal"].AsDecimal;

            List<RevertBundleItemDetail> bItems = new();
            foreach (var bItem in bundleItems)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == bItem.OrderItemId);
                if (item is null)
                {
                    Console.WriteLine($"\t[RevertBundleItemDiscountHandler] O produto do pedido {bItem.OrderItemId} no evento não foi encontrado no pedido {order.Id}.");
                    return Result<bool>.Failure(new List<Error>{ new("RevertBundleItemDiscountHandler.NotFound", $"O produto do pedido {bItem.OrderItemId} no evento não foi encontrado no pedido {order.Id}.") });
                }

                bItems.Add(new RevertBundleItemDetail(
                    bItem.OrderItemId,
                    bItem.ProductId,
                    bItem.Quantity,
                    bItem.AmountDiscountedPrice
                ));
            }

            var getDiscount = await _revertOrderUoW.Discounts.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertBundleItemDiscountHandler] Os dados do desconto '{discountName}' não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertBundleItemDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            Coupon? coupon = null;
            if (couponId is not null)
            {
                var getCoupon = await _revertOrderUoW.Coupons.Get(couponId.Value);
                if (getCoupon.IsFailure)
                {
                    Console.WriteLine($"\t[RevertBundleItemDiscountHandler] Os dados do cupom '{couponCode}' aplicado ao desconto de pacote não foram encontrados.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBundleItemDiscountHandler.NotFound", $"Os dados do cupom '{couponCode}' aplicado ao desconto de pacote não foram encontrados.") });
                }
                coupon = getCoupon.GetValue();
                coupon.SetUsed(false);
                if (coupon.Validate() is { IsFailure: true } result)
                {
                    Console.WriteLine($"\t[RevertBundleItemDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao desconto de pacote.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBundleItemDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao desconto de pacote.") });
                }
                var updateResult = await _revertOrderUoW.Coupons.Update(coupon);
                if (updateResult.IsFailure)
                {
                    Console.WriteLine($"\t[RevertBundleItemDiscountHandler] Falha ao remover o uso do cupom '{couponCode}' aplicado ao desconto de pacote.");
                    return Result<bool>.Failure(new List<Error> { new("RevertBundleItemDiscountHandler.UpdateFail", $"Falha ao remover o uso do cupom '{couponCode}' aplicado ao desconto de pacote.") });
                }
            }

            var publishEvent = order.RevertBundleItemDiscount(bundleId, bItems, discountId, discountName, discountType, couponId, couponCode, amountDiscountedTotal);
            Console.WriteLine($"\t[RevertBundleItemDiscountHandler] O desconto {discountName} foi removido do pedido {order.Id}, os produtos do pacote foram revertidos aos produtos originiais. Valor Revertido: {publishEvent.AmountRevertedTotal:C}. Novo Total {publishEvent.CurrentTotal:C}.");

            foreach (var bItem in bundleItems)
            {
                order.AddUnappliedDiscount(new DiscountInProcess(
                    discountId,
                    bItem.OrderItemId,
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
            }

            return Result<bool>.Success(true);
        }
    }
}
