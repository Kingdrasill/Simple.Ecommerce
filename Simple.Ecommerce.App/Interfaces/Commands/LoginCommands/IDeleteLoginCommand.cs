using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.LoginCommands
{
    public interface IDeleteLoginCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
