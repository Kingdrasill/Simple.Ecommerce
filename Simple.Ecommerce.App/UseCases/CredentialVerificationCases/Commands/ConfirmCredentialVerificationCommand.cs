using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;

namespace Simple.Ecommerce.App.UseCases.CredentialVerificationCases.Commands
{
    public class ConfirmCredentialVerificationCommand : IConfirmCredentialVerificationCommand
    {
        private readonly ICredentialVerificationRepository _repository;
        private readonly ILoginRepository _loginRepository;

        public ConfirmCredentialVerificationCommand(
            ICredentialVerificationRepository repository,
            ILoginRepository loginRepository
        )
        {
            _repository = repository;
            _loginRepository = loginRepository;
        }

        public async Task<Result<bool>> Execute(string token)
        {
            var getVertification = await _repository.GetByToken(token);
            if (getVertification.IsFailure)
            {
                return Result<bool>.Failure(getVertification.Errors!);
            }

            var verification = getVertification.GetValue();

            if (verification.ExpiresAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure(new List<Error> { new("ConfirmCredentialVerificationCommand.DateExpired.ExpiresAt", "O token já expirado!") });
            }

            if (verification.IsUsed)
            {
                return Result<bool>.Failure(new List<Error> { new("ConfirmCredentialVerificationCommand.AlredayUsed", "O token já foi usado!") });
            }

            var getLogin = await _loginRepository.Get(verification.LoginId);
            if (getLogin.IsFailure)
            {
                return Result<bool>.Failure(getLogin.Errors!);
            }

            var login = getLogin.GetValue();
            login.SetVerified();

            var updateLoginResult = await _loginRepository.Update(login);
            if (updateLoginResult.IsFailure)
            {
                return Result<bool>.Failure(updateLoginResult.Errors!);
            }

            verification.MarkAsUsed();

            var updateVerificationResult = await _repository.Update(verification);
            if (updateVerificationResult.IsFailure)
            {
                return Result<bool>.Failure(updateVerificationResult.Errors!);
            }

            return Result<bool>.Success(true);
        }
    }
}
