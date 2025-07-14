using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IRemovePhotoUserCommand
    {
        Task<Result<bool>> Execute(int userId);
    }
}
