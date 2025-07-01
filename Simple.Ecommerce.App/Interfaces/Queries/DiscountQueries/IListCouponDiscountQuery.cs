using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListCouponDiscountQuery
    {
        Task<Result<List<CouponResponse>>> Execute();
    }
}
