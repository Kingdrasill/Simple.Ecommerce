using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly ICreateUserUnitOfWork _createUserUoW;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEmailService _emailService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateUserCommand(
            ICreateUserUnitOfWork createUserUoW,
            ICryptographyService cryptographyService,
            IEmailService emailService,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _createUserUoW = createUserUoW;
            _cryptographyService = cryptographyService;
            _emailService = emailService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserResponse>> Execute(UserRequest request)
        {
            await _createUserUoW.BeginTransaction();
            try
            {
                // Checkando se usuário com o mesmo email já existe
                var getUser = await _createUserUoW.Users.GetByEmail(request.Email);
                if (getUser.IsSuccess)
                {
                    throw new ResultException(new Error("CreateUserCommand.Conflict.Email", "Um usuário já cadastrou com este email!"));
                }

                // Checkando se usuário com o mesmo númerio já existe
                getUser = await _createUserUoW.Users.GetByPhoneNumber(request.PhoneNumber);
                if (getUser.IsSuccess)
                {
                    throw new ResultException(new Error("CreateUserCommand.Conflict.PhoneNumber", "Um usuário já cadastrou com este telefone!"));
                }

                // Hashing a senha
                var hashedPasswordResult = _cryptographyService.HashPassword(request.Password);
                if (hashedPasswordResult.IsFailure)
                {
                    throw new ResultException(hashedPasswordResult.Errors!);
                }

                // Criando a instância do usuário
                var userInstance = new User().Create(
                    request.Id,
                    request.Name,
                    request.Email,
                    request.PhoneNumber,
                    hashedPasswordResult.GetValue()
                );
                if (userInstance.IsFailure)
                {
                    throw new ResultException(userInstance.Errors!);
                }

                var createUserResult = await _createUserUoW.Users.Create(userInstance.GetValue(), true);
                if (createUserResult.IsFailure)
                {
                    throw new ResultException(createUserResult.Errors!);
                }
                var user = createUserResult.GetValue();

                // Criando instância de login por email
                var loginEmailInstance = new Login().Create(
                    0,
                    null,
                    user,
                    request.Email,
                    hashedPasswordResult.GetValue(),
                    CredentialType.Email,
                    null
                );
                if (loginEmailInstance.IsFailure)
                {
                    throw new ResultException(loginEmailInstance.Errors!);
                }

                var createLoginEmailResult = await _createUserUoW.Logins.Create(loginEmailInstance.GetValue(), true);
                if (createLoginEmailResult.IsFailure)
                {
                    throw new ResultException(createLoginEmailResult.Errors!);
                }

                // Crianda instância para verificação do email de login
                var credentialVerificationInstance = new CredentialVerification().Create(
                    0,
                    null,
                    loginEmailInstance.GetValue(),
                    Guid.NewGuid().ToString("N"),
                    DateTime.UtcNow.AddHours(24),
                    null,
                    null
                );
                if (credentialVerificationInstance.IsFailure)
                {
                    throw new ResultException(credentialVerificationInstance.Errors!);
                }

                var createCredentialVerificationaResult = await _createUserUoW.CredentialVerifications.Create(credentialVerificationInstance.GetValue(), true);
                if (createCredentialVerificationaResult.IsFailure)
                {
                    throw new ResultException(createCredentialVerificationaResult.Errors!);
                }

                // Criando instância de login por número
                var loginPhoneNumberInstance = new Login().Create(
                    0,
                    null,
                    user,
                    request.PhoneNumber,
                    hashedPasswordResult.GetValue(),
                    CredentialType.Phone,
                    null
                );
                if (loginPhoneNumberInstance.IsFailure)
                {
                    throw new ResultException(loginPhoneNumberInstance.Errors!);
                }

                var createLoginPhoneNumberResult = await _createUserUoW.Logins.Create(loginPhoneNumberInstance.GetValue(), true);
                if (createLoginPhoneNumberResult.IsFailure)
                {
                    throw new ResultException(createLoginPhoneNumberResult.Errors!);
                }

                await _createUserUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<User>();
                    _cacheHandler.SetItemStale<Login>();
                }

                await _emailService.SendEmailVerification(request.Email, credentialVerificationInstance.GetValue().Token);

                var response = new UserResponse(
                    user.Id,
                    user.Name,
                    createLoginEmailResult.GetValue().Credential,
                    createLoginPhoneNumberResult.GetValue().Credential
                );

                return Result<UserResponse>.Success(response);
            }
            catch (ResultException rex)
            {
                await _createUserUoW.Rollback();
                return Result<UserResponse>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _createUserUoW.Rollback();
                return Result<UserResponse>.Failure(new List<Error> { new("CreateUserCommand.Unknown", ex.Message) });
            }
        }
    }
}
