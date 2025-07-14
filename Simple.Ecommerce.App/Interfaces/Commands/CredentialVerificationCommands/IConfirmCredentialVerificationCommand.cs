using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands
{
    public interface IConfirmCredentialVerificationCommand
    {
        Task<Result<bool>> Execute(string token);
    }
}
