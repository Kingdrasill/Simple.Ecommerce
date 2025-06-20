using Simple.Ecommerce.Contracts.LoginContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.LoginCommands
{
    public interface IAuthenticateLoginCommand
    {
        Task<Result<AuthenticateResponse>> Execute(AuthenticateRequest request);
    }
}
