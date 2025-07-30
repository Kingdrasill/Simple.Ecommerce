namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemRequest
    (
        int Quantity,
        int ProductId,
        int OrderId,
        int? ProductDiscountId = null,
        bool Override = false
    );
}
