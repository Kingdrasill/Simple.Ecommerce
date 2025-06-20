namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemRequest
    (
        int Quantity,
        decimal Price,
        int ProductId,
        int OrderId
    );
}
