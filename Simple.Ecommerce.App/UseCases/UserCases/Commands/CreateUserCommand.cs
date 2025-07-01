using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly ILoginRepository _loginRepository;
        private readonly ICredentialVerificationRepository _credentialVerificationRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEmailService _emailService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateUserCommand(
            IUserRepository repository,
            ILoginRepository loginRepository,
            ICredentialVerificationRepository credentialVerificationRepository,
            ISaverTransectioner unityOfWork,
            ICryptographyService cryptographyService,
            IEmailService emailService,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _loginRepository = loginRepository;
            _credentialVerificationRepository = credentialVerificationRepository;
            _saverOrTransectioner = unityOfWork;
            _cryptographyService = cryptographyService;
            _emailService = emailService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserResponse>> Execute(UserRequest request)
        {
            await _saverOrTransectioner.BeginTransaction();
            try
            {
                // Checkando se usuário com o mesmo email já existe
                var getUser = await _repository.GetByEmail(request.Email);
                if (getUser.IsSuccess)
                {
                    throw new ResultException(new Error("CreateUserCommand.Conflict.Email", "Um usuário já cadastrou com este email!"));
                }

                // Checkando se usuário com o mesmo númerio já existe
                getUser = await _repository.GetByPhoneNumber(request.PhoneNumber);
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

                var createUserResult = await _repository.Create(userInstance.GetValue());
                if (createUserResult.IsFailure)
                {
                    throw new ResultException(createUserResult.Errors!);
                }

                var user = createUserResult.GetValue();

                // Criando instância de login por email
                var loginEmailInstance = new Login().Create(
                    0,
                    user,
                    request.Email,
                    hashedPasswordResult.GetValue(),
                    CredentialType.Email
                );
                if (loginEmailInstance.IsFailure)
                {
                    throw new ResultException(loginEmailInstance.Errors!);
                }

                var createLoginEmailResult = await _loginRepository.Create(loginEmailInstance.GetValue());
                if (createLoginEmailResult.IsFailure)
                {
                    throw new ResultException(createLoginEmailResult.Errors!);
                }

                // Crianda instância para verificação do email de login
                var credentialVerificationInstance = new CredentialVerification().Create(
                    0,
                    loginEmailInstance.GetValue(),
                    Guid.NewGuid().ToString("N"),
                    DateTime.UtcNow.AddHours(24)
                );
                if (credentialVerificationInstance.IsFailure)
                {
                    throw new ResultException(credentialVerificationInstance.Errors!);
                }

                var createCredentialVerificationaResult = await _credentialVerificationRepository.Create(credentialVerificationInstance.GetValue());
                if (createCredentialVerificationaResult.IsFailure)
                {
                    throw new ResultException(createCredentialVerificationaResult.Errors!);
                }

                // Criando instância de login por número
                var loginPhoneNumberInstance = new Login().Create(
                    0,
                    user,
                    request.PhoneNumber,
                    hashedPasswordResult.GetValue(),
                    CredentialType.Phone
                );
                if (loginPhoneNumberInstance.IsFailure)
                {
                    throw new ResultException(loginPhoneNumberInstance.Errors!);
                }

                var createLoginPhoneNumberResult = await _loginRepository.Create(loginPhoneNumberInstance.GetValue());
                if (createLoginPhoneNumberResult.IsFailure)
                {
                    throw new ResultException(createLoginPhoneNumberResult.Errors!);
                }

                await _saverOrTransectioner.Commit();

                await _emailService.SendEmailVerification(request.Email, credentialVerificationInstance.GetValue().Token);

                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<User>();
                    _cacheHandler.SetItemStale<Login>();
                }

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
                await _saverOrTransectioner.Rollback();
                return Result<UserResponse>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<UserResponse>.Failure(new List<Error> { new("CreateUserCommand.Unknown", ex.Message) });
            }
        }
    }
}
