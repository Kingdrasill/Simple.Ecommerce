namespace Simple.Ecommerce.Contracts.CouponContracts
{
    public record CouponRequest
    (
        int Id,
        string Code,
        bool IsUsed,
        DateTime CreatedAt,
        DateTime ExpirationAt,
        DateTime? UsedAt,
        int DiscountId
    );
}
