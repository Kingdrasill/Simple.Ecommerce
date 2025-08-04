using Simple.Ecommerce.Contracts.DiscountContracts;

namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemWithDiscountDTO
    (
        int Id,
        int ProductId,
        string ProductName,
        int Quantity,
        decimal Price,
        DiscountDTO? Discount
    );
}
