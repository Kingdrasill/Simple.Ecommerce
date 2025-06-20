using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries
{
    public interface IListCategoryQuery
    {
        Task<Result<List<CategoryResponse>>> Execute();
    }
}
