namespace Simple.Ecommerce.Contracts.CouponContracts
{
    public record CouponBatchRequest
    (
        int Quantity,
        DateTime ExpirationAt,
        int DiscountId,
        string? Prefix = null
    );
}
