using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.DiscountContracts
{
    public record DiscountItemDTO
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
        List<DiscountTierResponse>? Tiers
    );
}
