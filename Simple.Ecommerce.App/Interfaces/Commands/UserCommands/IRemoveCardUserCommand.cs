using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IRemoveCardUserCommand
    {
        Task<Result<bool>> Execute(int userCardId);
    }
}
