using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.CredentialVerificationCases.Commands
{
    public class ConfirmCredentialVerificationCommand : IConfirmCredentialVerificationCommand
    {
        private readonly IConfirmCredentialVerificationUnitOfWork _confirmCredentialVerificationUoW;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ConfirmCredentialVerificationCommand(
            IConfirmCredentialVerificationUnitOfWork confirmCredentialVerificationUoW,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _confirmCredentialVerificationUoW = confirmCredentialVerificationUoW;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(string token)
        {
            await _confirmCredentialVerificationUoW.BeginTransaction();
            try
            {
                var getVertification = await _confirmCredentialVerificationUoW.CredentialVerifications.GetByToken(token);
                if (getVertification.IsFailure)
                {
                    throw new ResultException(getVertification.Errors!);
                }
                var verification = getVertification.GetValue();

                var getLogin = await _confirmCredentialVerificationUoW.Logins.Get(verification.LoginId);
                if (getLogin.IsFailure)
                {
                    throw new ResultException(getLogin.Errors!);
                }
                var login = getLogin.GetValue();

                if (verification.ExpiresAt < DateTime.UtcNow)
                {
                    throw new ResultException(new Error("ConfirmCredentialVerificationCommand.tokenExpired", "O token já expirado!"));
                }
                if (verification.IsUsed)
                {
                    throw new ResultException(new Error("ConfirmCredentialVerificationCommand.AlredayUsed", "O token já foi usado!"));
                }

                verification.MarkAsUsed();
                if (verification.Validate() is { IsFailure: true } vResult)
                {
                    throw new ResultException(vResult.Errors!);
                }

                var updateVerificationResult = await _confirmCredentialVerificationUoW.CredentialVerifications.Update(verification, true);
                if (updateVerificationResult.IsFailure)
                {
                    throw new ResultException(updateVerificationResult.Errors!);
                }

                login.SetVerified();
                if (login.Validate() is { IsFailure: true } lResult)
                {
                    throw new ResultException(lResult.Errors!);
                }

                var updateLoginResult = await _confirmCredentialVerificationUoW.Logins.Update(login, true);
                if (updateLoginResult.IsFailure)
                {
                    throw new ResultException(updateLoginResult.Errors!);
                }

                await _confirmCredentialVerificationUoW.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<Login>();

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _confirmCredentialVerificationUoW.Rollback();
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _confirmCredentialVerificationUoW.Rollback();
                return Result<bool>.Failure(new List<Error> { new("ConfirmCredentialVerificationCommand.Unknown", ex.Message) });
            }

        }
    }
}
