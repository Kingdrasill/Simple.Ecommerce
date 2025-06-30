namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemRequest
    (
        int Quantity,
        int ProductId,
        int OrderId,
        int? DiscountId = null,
        bool Override = false
    );
}
