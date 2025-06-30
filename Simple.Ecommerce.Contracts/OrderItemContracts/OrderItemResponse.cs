namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemResponse
    (
        int Id,
        decimal Price,
        int Quantity,
        int ProductId,
        int OrderId,
        int? DiscountId
    );
}
