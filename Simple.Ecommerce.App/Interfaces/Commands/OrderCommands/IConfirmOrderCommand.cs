using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IConfirmOrderCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
