using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries
{
    public interface IListCategoryQuery
    {
        Task<Result<List<CategoryResponse>>> Execute();
    }
}
