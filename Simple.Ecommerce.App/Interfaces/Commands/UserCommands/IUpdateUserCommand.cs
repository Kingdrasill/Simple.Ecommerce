using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IUpdateUserCommand
    {
        Task<Result<UserResponse>> Execute(UserRequest request);
    }
}
