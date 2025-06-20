using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUseCouponDiscountCommand
    {
        Task<Result<bool>> Execute(string code);
    }
}
