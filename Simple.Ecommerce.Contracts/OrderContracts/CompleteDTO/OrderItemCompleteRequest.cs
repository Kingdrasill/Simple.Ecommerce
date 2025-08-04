namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record OrderItemCompleteRequest
    (
        int ProductId,
        int Quantity,
        string? CouponCode = null,
        int? ProductDiscountId = null
    );
}
