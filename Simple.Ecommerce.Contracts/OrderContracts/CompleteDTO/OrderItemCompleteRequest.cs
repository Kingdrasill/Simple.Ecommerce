namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record OrderItemCompleteRequest
    (
        int ProductId,
        int Quantity,
        int? ProductDiscountId = null
    );
}
