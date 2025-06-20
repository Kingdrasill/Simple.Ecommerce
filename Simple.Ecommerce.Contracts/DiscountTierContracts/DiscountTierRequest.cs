namespace Simple.Ecommerce.Contracts.DiscountTierContracts
{
    public record DiscountTierRequest
    (
        int MinQuality,
        decimal Value,
        int DiscountId,
        int Id = 0
    );
}
