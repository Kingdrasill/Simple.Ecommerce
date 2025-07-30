namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemWithDiscountDTO
    (
        int Id,
        int ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        OrderItemDiscountDTO? Discount
    );
}
