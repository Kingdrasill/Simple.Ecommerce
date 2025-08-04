using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.CouponsValidationHandler
{
    public class CouponsValidationHandler : BaseOrderProcessingHandler
    {
        public CouponsValidationHandler() : base() { }

        public override Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            var unAppliedCoupons = orderInProcess.UnAppliedDiscounts.Where(uad => uad.Coupon is not null).ToList();
            foreach (var unAppliedCoupon in unAppliedCoupons)
            {
                var coupon = unAppliedCoupon.Coupon!;

                if (coupon.IsUsed)
                {
                    throw new ResultException(new Error("CouponsValidationHandler.AlreadyUsed", $"O cupom {coupon.Code} já foi utilizado!"));
                }
                if (coupon.ExpirationAt < DateTime.UtcNow)
                {
                    throw new ResultException(new Error("CouponsValidationHandler.AlreadyExpired", $"O cupom {coupon.Code} já expirou!"));
                }
                if (coupon.DiscountId != unAppliedCoupon.Id)
                {
                    throw new ResultException(new Error("CouponsValidationHandler.Conflict.Discount", $"O cupom {coupon.Code} para o desconto {coupon.DiscountId} foi passado para o desconto {unAppliedCoupon.Id}!"));
                }
            }

            var bundleDiscounts = orderInProcess.UnAppliedDiscounts.Where(uad => uad.DiscountType == DiscountType.Bundle);
            var groupedBundles = bundleDiscounts.GroupBy(uad => uad.Id);
            foreach (var bundleGroup in groupedBundles)
            {
                var couponIds = bundleGroup
                    .Where(bg => bg.Coupon is not null)
                    .Select(bg => bg.Coupon!.Id)
                    .Distinct()
                    .ToList();
                if (couponIds.Count > 1)
                {
                    throw new ResultException(new Error("CouponsValidationHandler.Conflict.Coupon", $"Itens do pacote {bundleGroup.First().Name} têm múltiplos cupons aplicados ({couponIds.Count}). Todos os itens de um pacote devem compartilhar o mesmo cupom."));
                }
            }

            return base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
