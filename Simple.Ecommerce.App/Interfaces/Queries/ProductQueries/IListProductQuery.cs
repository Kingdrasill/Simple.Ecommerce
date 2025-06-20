using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IListProductQuery
    {
        Task<Result<List<ProductResponse>>> Execute();
    }
}
