namespace Simple.Ecommerce.Contracts.DiscountTierContracts
{
    public record DiscountTierRequest
    (
        string Name,
        int MinQuality,
        decimal Value,
        int DiscountId,
        int Id = 0
    );
}
