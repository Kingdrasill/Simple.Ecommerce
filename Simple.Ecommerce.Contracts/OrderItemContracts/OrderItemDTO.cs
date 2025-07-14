using Simple.Ecommerce.Contracts.DiscountContracts;

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
