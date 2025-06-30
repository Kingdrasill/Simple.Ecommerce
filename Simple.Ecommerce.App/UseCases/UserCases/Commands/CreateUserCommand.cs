using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly ILoginRepository _loginRepository;
        private readonly ICredentialVerificationRepository _credentialVerificationRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEmailService _emailService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateUserCommand(
            IUserRepository repository,
            ILoginRepository loginRepository,
            ICredentialVerificationRepository credentialVerificationRepository,
            ICryptographyService cryptographyService,
            IEmailService emailService,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _loginRepository = loginRepository;
            _credentialVerificationRepository = credentialVerificationRepository;
            _cryptographyService = cryptographyService;
            _emailService = emailService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserResponse>> Execute(UserRequest request)
        {
            var getUser = await _repository.GetByEmail(request.Email);
            if (getUser.IsSuccess)
            {
                return Result<UserResponse>.Failure(new List<Error> { new("CreateUserCommand.Conflict.Email", "Um usuário já cadastrou com este email!") });
            }

            getUser = await _repository.GetByPhoneNumber(request.PhoneNumber);
            if (getUser.IsSuccess)
            {
                return Result<UserResponse>.Failure(new List<Error> { new("CreateUserCommand.Conflict.PhoneNumber", "Um usuário já cadastrou com este telefone!") });
            }

            var hashedPasswordResult = _cryptographyService.HashPassword(request.Password);

            var userInstance = new User().Create(
                request.Id,
                request.Name,
                request.Email,
                request.PhoneNumber,
                hashedPasswordResult.GetValue()
            );
            if (userInstance.IsFailure)
            {
                return Result<UserResponse>.Failure(userInstance.Errors!);
            }

            var createUserResult = await _repository.Create(userInstance.GetValue());
            if (createUserResult.IsFailure)
            {
                return Result<UserResponse>.Failure(createUserResult.Errors!);
            }

            var user = createUserResult.GetValue();
            if (_useCache.Use)
                _cacheHandler.SetItemStale<User>();

            var loginEmailInstance = new Login().Create(
                0,
                user.Id,
                request.Email,
                hashedPasswordResult.GetValue(),
                CredentialType.Email
            );
            if (loginEmailInstance.IsFailure)
            {
                return Result<UserResponse>.Failure(loginEmailInstance.Errors!);
            }

            var createLoginEmailResult = await _loginRepository.Create(loginEmailInstance.GetValue());
            if (createLoginEmailResult.IsFailure)
            {
                return Result<UserResponse>.Failure(createLoginEmailResult.Errors!);
            }

            var credentialVerificationInstance = new CredentialVerification().Create(
                0,
                loginEmailInstance.GetValue().Id,
                Guid.NewGuid().ToString("N"),
                DateTime.UtcNow.AddHours(24)
            );
            if (credentialVerificationInstance.IsFailure)
            {
                return Result<UserResponse>.Failure(credentialVerificationInstance.Errors!);
            }

            var createCredentialVerificationaResult = await _credentialVerificationRepository.Create(credentialVerificationInstance.GetValue());
            if (createCredentialVerificationaResult.IsFailure)
            {
                return Result<UserResponse>.Failure(createCredentialVerificationaResult.Errors!);
            }
            await _emailService.SendEmailVerification(request.Email, credentialVerificationInstance.GetValue().Token);

            var loginPhoneNumberInstance = new Login().Create(
                0,
                user.Id,
                request.PhoneNumber,
                hashedPasswordResult.GetValue(),
                CredentialType.Phone
            );
            if (loginPhoneNumberInstance.IsFailure)
            {
                return Result<UserResponse>.Failure(loginPhoneNumberInstance.Errors!);
            }

            var createLoginPhoneNumberResult = await _loginRepository.Create(loginPhoneNumberInstance.GetValue());
            if (createLoginPhoneNumberResult.IsFailure)
            {
                return Result<UserResponse>.Failure(createLoginPhoneNumberResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Login>();

            var response = new UserResponse(
                user.Id,
                user.Name,
                createLoginEmailResult.GetValue().Credential,
                createLoginPhoneNumberResult.GetValue().Credential
            );

            return Result<UserResponse>.Success(response);
        }
    }
}
