using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IGetDiscountTierDiscountQuery
    {
        Task<Result<DiscountTierResponse>> Execute(int id);
    }
}
