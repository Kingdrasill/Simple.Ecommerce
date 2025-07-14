using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListTierDiscountQuery
    {
        Task<Result<List<DiscountTierResponse>>> Execute();
    }
}
