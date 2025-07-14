using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IGetTierDiscountQuery
    {
        Task<Result<DiscountTierResponse>> Execute(int id);
    }
}
