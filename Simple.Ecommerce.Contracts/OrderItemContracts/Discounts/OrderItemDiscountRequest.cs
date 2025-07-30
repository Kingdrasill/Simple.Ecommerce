namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemDiscountRequest
    (
        int OrderId,
        int ProductId,
        int? ProductDiscountId = null
    );
}
