namespace Simple.Ecommerce.Contracts.CouponContracts
{
    public record CouponResponse
    (
        int Id,
        string Code,
        bool IsUsed,
        DateTime CreatedAt,
        DateTime ExpirationAt,
        DateTime? UsedAt,
        int? DiscountId = null
    );
}
