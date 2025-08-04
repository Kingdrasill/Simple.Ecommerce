namespace Simple.Ecommerce.Contracts.OrderContracts.Discounts
{
    public record OrderDiscountRequest
    (
        int OrderId,
        string? CouponCode = null,
        int? DiscountId = null
    );
}
