using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IGetDiscountQuery
    {
        Task<Result<DiscountResponse>> Execute(int id);
    }
}
