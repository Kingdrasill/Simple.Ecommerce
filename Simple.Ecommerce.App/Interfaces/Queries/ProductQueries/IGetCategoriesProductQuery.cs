using Simple.Ecommerce.Contracts.ProductCategoryContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetCategoriesProductQuery
    {
        Task<Result<ProductCategoriesDTO>> Execute(int productId);
    }
}
