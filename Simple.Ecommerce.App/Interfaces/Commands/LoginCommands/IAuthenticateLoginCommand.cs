using Simple.Ecommerce.Contracts.LoginContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.LoginCommands
{
    public interface IAuthenticateLoginCommand
    {
        Task<Result<AuthenticateResponse>> Execute(AuthenticateRequest request);
    }
}
