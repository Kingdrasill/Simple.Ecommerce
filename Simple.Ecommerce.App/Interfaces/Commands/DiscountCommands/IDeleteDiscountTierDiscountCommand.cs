using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteDiscountTierDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
