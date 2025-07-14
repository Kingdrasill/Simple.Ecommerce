namespace Simple.Ecommerce.Contracts.OrderItemContracts
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
