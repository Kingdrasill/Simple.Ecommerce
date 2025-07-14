using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
