using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemDiscountDTO
    (
        int OrderItemId,
        int DiscountId,
        string DiscountName,
        DiscountType DiscountType,
        DiscountScope DiscountScope,
        DiscountValueType? DiscountValueType,
        decimal? Value,
        DateTime? ValidFrom,
        DateTime? ValidTo,
        bool IsActive
    );
}
