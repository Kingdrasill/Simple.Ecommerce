using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;

namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemDTO
    (
        int Id,
        int ProductId,
        string ProductName,
        int Quantity,
        decimal Price,
        DiscountItemDTO? AppliedDiscount
    );
}
