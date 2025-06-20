using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteDiscountBundleItemDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}