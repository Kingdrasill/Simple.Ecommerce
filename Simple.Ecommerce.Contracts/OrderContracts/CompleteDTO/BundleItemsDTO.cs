using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record BundleItemsDTO
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
        CouponItemDTO? Coupon,
        List<BundleItemDTO> BundleItems
    );
}
