using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IDeletePhotoUserCommand
    {
        Task<Result<bool>> Execute(int userId);
    }
}
