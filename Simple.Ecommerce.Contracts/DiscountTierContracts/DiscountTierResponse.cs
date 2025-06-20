namespace Simple.Ecommerce.Contracts.DiscountTierContracts
{
    public record DiscountTierResponse
    (
        int Id,
        int MinQuality,
        decimal Value,
        int? DiscountId = null
    );
}
