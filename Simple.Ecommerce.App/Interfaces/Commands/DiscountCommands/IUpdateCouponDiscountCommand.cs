using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUpdateCouponDiscountCommand
    {
        Task<Result<CouponResponse>> Execute(CouponRequest request);
    }
}
