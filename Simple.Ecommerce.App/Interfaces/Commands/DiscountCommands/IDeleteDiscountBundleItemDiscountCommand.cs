using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteDiscountBundleItemDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}