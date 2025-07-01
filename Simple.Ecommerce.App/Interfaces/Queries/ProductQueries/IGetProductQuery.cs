using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetProductQuery
    {
        Task<Result<ProductResponse>> Execute(int id);
    }
}
