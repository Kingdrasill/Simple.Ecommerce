using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
