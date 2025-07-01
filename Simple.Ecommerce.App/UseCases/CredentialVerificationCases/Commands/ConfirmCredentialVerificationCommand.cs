using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.UseCases.CredentialVerificationCases.Commands
{
    public class ConfirmCredentialVerificationCommand : IConfirmCredentialVerificationCommand
    {
        private readonly ICredentialVerificationRepository _repository;
        private readonly ILoginRepository _loginRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;

        public ConfirmCredentialVerificationCommand(
            ICredentialVerificationRepository repository,
            ILoginRepository loginRepository,
            ISaverTransectioner unityOfWork
        )
        {
            _repository = repository;
            _loginRepository = loginRepository;
            _saverOrTransectioner = unityOfWork;
        }

        public async Task<Result<bool>> Execute(string token)
        {
            await _saverOrTransectioner.BeginTransaction();

            try
            {
                var getVertification = await _repository.GetByToken(token);
                if (getVertification.IsFailure)
                {
                    throw new ResultException(getVertification.Errors!);
                }

                var verification = getVertification.GetValue();

                if (verification.ExpiresAt < DateTime.UtcNow)
                {
                    throw new ResultException(new Error("ConfirmCredentialVerificationCommand.tokenExpired", "O token já expirado!"));
                }

                if (verification.IsUsed)
                {
                    throw new ResultException(new Error("ConfirmCredentialVerificationCommand.AlredayUsed", "O token já foi usado!"));
                }

                var getLogin = await _loginRepository.Get(verification.LoginId);
                if (getLogin.IsFailure)
                {
                    throw new ResultException(getLogin.Errors!);
                }

                var login = getLogin.GetValue();
                login.SetVerified();

                var updateLoginResult = await _loginRepository.Update(login);
                if (updateLoginResult.IsFailure)
                {
                    throw new ResultException(updateLoginResult.Errors!);
                }

                verification.MarkAsUsed();

                var updateVerificationResult = await _repository.Update(verification);
                if (updateVerificationResult.IsFailure)
                {
                    throw new ResultException(updateVerificationResult.Errors!);
                }

                await _saverOrTransectioner.Commit();

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<bool>.Failure(new List<Error> { new("ConfirmCredentialVerificationCommand.Unknown", ex.Message) });
            }

        }
    }
}
