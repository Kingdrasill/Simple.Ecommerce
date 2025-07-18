using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderContracts.Discounts
{
    public record OrderDiscountDTO
    (
        int OrderId,
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
