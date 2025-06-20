using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IAddAddressUserCommand
    {
        Task<Result<bool>> Execute(UserAddressRequest request);
    }
}
