namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemResponse
    (
        int Id,
        int ProductId,
        decimal Price,
        int Quantity,
        int OrderId
    );
}
