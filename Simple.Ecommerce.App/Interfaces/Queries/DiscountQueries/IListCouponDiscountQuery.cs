using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListCouponDiscountQuery
    {
        Task<Result<List<CouponResponse>>> Execute();
    }
}
