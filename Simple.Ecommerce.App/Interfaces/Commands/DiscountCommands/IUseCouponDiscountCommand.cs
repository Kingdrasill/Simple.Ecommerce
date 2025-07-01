using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUseCouponDiscountCommand
    {
        Task<Result<bool>> Execute(string code);
    }
}
