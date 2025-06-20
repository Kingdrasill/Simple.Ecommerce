namespace Simple.Ecommerce.Contracts.ProductCategoryContracts
{
    public record ProductCategoryDTO
    (
        int Id,
        int ProductId,
        string ProductName,
        int CategoryId,
        string CategoryName
    );
}
