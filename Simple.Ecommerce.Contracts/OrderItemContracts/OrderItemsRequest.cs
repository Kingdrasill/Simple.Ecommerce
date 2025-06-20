namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemsRequest
    (
        int OrderId,
        List<OrderItemRequest> OrderItems
    );
}
