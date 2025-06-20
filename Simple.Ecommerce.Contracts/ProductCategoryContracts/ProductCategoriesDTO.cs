using Simple.Ecommerce.Contracts.CategoryContracts;

namespace Simple.Ecommerce.Contracts.ProductCategoryContracts
{
    public record ProductCategoriesDTO
    (
        int ProductId,
        string ProductName,
        List<CategoryResponse> Categories
    );
}
