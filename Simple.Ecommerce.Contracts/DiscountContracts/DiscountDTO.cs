using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.DiscountContracts
{
    public record DiscountDTO
    (
        int Id,
        string Name,
        DiscountType DiscountType,
        DiscountScope DiscountScope,
        DiscountValueType? DiscountValueType,
        decimal? Value,
        DateTime? ValidFrom,
        DateTime? ValidTo,
        bool IsActive,
        CouponDTO? Coupon
    );
}
