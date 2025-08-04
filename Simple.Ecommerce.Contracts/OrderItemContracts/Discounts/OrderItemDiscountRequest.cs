namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemDiscountRequest
    (
        int OrderId,
        int ProductId,
        string? CouponCode = null,
        int? ProductDiscountId = null
    );
}
