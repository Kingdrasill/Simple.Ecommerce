using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries
{
    public interface IGetCategoryQuery
    {
        Task<Result<CategoryResponse>> Execute(int id, bool NoTracking = true);
    }
}
