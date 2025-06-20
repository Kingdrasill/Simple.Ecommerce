using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries
{
    public interface IGetCategoryQuery
    {
        Task<Result<CategoryResponse>> Execute(int id, bool NoTracking = true);
    }
}
