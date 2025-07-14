namespace Simple.Ecommerce.Contracts.DiscountTierContracts
{
    public record DiscountTierResponse
    (
        int Id,
        string Name,
        int MinQuality,
        decimal Value,
        int? DiscountId = null
    );
}
