using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface ICreateUserCommand
    {
        Task<Result<UserResponse>> Execute(UserRequest request);
    }
}
