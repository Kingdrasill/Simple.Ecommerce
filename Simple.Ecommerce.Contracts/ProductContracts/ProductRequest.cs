namespace Simple.Ecommerce.Contracts.ProductContracts
{
    public record ProductRequest
    (
        string Name, 
        decimal Price,
        string Description,
        int Stock,
        int Id = 0
    );
}
