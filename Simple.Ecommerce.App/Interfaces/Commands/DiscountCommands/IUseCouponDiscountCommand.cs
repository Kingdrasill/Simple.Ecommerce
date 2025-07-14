using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUseCouponDiscountCommand
    {
        Task<Result<bool>> Execute(string code);
    }
}
