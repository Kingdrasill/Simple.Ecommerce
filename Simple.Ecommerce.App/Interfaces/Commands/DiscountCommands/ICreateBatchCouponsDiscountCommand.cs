using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateBatchCouponsDiscountCommand
    {
        Task<Result<List<CouponResponse>>> Execute(CouponBatchRequest request);
    }
}
