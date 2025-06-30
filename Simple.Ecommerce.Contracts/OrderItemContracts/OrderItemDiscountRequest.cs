namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemDiscountRequest
    (
        int OrderId,
        int ProductId,
        int? DiscountId = null
    );
}
