using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IRemoveAddressUserCommand
    {
        Task<Result<bool>> Execute(int userAddressId);
    }
}
