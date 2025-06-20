using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetProductQuery
    {
        Task<Result<ProductResponse>> Execute(int id, bool NoTracking = true);
    }
}
