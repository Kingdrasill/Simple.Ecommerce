namespace Simple.Ecommerce.Contracts.ProductContracts
{
    public record ProductResponse
    (
        int Id,
        string Name,
        decimal Price,
        string Description,
        int Stock
    );
}
