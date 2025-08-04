namespace Simple.Ecommerce.Contracts.CouponContracts
{
    public record CouponDTO
    (
        int Id,
        int DiscountId,
        string Code,
        DateTime ExpirationAt,
        bool IsUsed
    );
}
