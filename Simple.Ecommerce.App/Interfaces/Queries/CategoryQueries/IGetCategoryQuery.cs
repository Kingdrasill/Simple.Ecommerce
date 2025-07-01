using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries
{
    public interface IGetCategoryQuery
    {
        Task<Result<CategoryResponse>> Execute(int id, bool NoTracking = true);
    }
}
