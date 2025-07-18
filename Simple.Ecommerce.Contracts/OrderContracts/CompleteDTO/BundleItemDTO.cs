namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record BundleItemDTO
    (
        int Id,
        int ProductId,
        string ProductName,
        int Quantity,
        decimal Price
    );
}
