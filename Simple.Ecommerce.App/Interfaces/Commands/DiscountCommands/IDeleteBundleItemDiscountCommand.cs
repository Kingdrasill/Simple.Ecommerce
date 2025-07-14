using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteBundleItemDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}