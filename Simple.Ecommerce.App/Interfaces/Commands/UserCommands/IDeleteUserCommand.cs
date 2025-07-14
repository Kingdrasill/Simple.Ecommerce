using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IDeleteUserCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
