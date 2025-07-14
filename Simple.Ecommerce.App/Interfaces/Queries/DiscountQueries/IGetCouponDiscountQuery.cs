using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IGetCouponDiscountQuery
    {
        Task<Result<CouponResponse>> Execute(int id);
    }
}
