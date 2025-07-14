using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IAddCardUserCommand
    {
        Task<Result<bool>> Execute(UserCardRequest request);
    }
}
