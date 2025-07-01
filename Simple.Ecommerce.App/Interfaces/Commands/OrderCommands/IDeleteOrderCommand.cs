using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IDeleteOrderCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
