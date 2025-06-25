using Simple.Ecommerce.App.Interfaces.Commands.LoginCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.LoginContracts;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.Authentication;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;

namespace Simple.Ecommerce.App.UseCases.LoginCases.Commands
{
    public class AuthenticateLoginCommand : IAuthenticateLoginCommand
    {
        private readonly ILoginRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AuthenticateLoginCommand(
            ILoginRepository repository, 
            IUserRepository userRepository, 
            ICryptographyService cryptographyService,
            IJwtAuthenticationService jwtAuthenticationService,
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _cryptographyService = cryptographyService;
            _jwtAuthenticationService = jwtAuthenticationService;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<AuthenticateResponse>> Execute(AuthenticateRequest request)
        {
            var authenticateResult = await AuthenticateLogin(request.Credential, request.Password);
            if (authenticateResult.IsFailure)
            {
                return Result<AuthenticateResponse>.Failure(authenticateResult.Errors!);        
            }

            var login = authenticateResult.GetValue();
            //if (!login.IsVerified)
            //{
            //    return Result<AuthenticateResponse>.Failure(new List<Error> { new Error("Locked", "A credencial usada para este login não foi verificada!") });
            //}

            var getUser = await GetUser(login);
            if (getUser.IsFailure)
            {
                return Result<AuthenticateResponse>.Failure(getUser.Errors!);
            }

            var getToken = _jwtAuthenticationService.GenerateJwtToken(getUser.GetValue(), login);
            if (getToken.IsFailure)
            {
                return Result<AuthenticateResponse>.Failure(getToken.Errors!);
            }

            return Result<AuthenticateResponse>.Success(
                new AuthenticateResponse(getToken.GetValue().Value, getToken.GetValue().Expiration)
            );
        }

        private async Task<Result<User>> GetUser(Login login)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<User, User>(login.UserId, cache =>
                    new User().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToString(cache["Email"])!,
                        Convert.ToString(cache["PhoneNumber"])!,
                        Convert.ToString(cache["Password"])!
                    ).GetValue());
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<User>(
                login.UserId,
                async (id) => await _userRepository.Get(id));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<User>();

            return repoResponse;
        }

        private async Task<Result<Login>> AuthenticateLogin(string credential, string password)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCacheByProperty<Login, LoginResponse>(nameof(Login.Credential), credential, cache => 
                    new LoginResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["UserId"]),
                        Convert.ToString(cache["Credential"])!,
                        Convert.ToString(cache["Password"])!,
                        (CredentialType)Convert.ToInt32(cache["Type"]),
                        Convert.ToBoolean(cache["IsVerified"])
                    ));
                if (cacheResponse.IsSuccess)
                {
                    var loginResponse = cacheResponse.GetValue();
                    if (_cryptographyService.VerifyPassword(password, loginResponse.Password).IsFailure)
                    {
                        return Result<Login>.Failure(new List<Error> { new("Forbidden", "Credencial ou senha incorretos!") });
                    }
                    var login = new Login().Create(
                        loginResponse.Id,
                        loginResponse.UserId,
                        loginResponse.Credential,
                        loginResponse.Password,
                        loginResponse.Type
                    ).GetValue();
                    if (loginResponse.IsVerified)
                        login.SetVerified();
                    return Result<Login>.Success(login);
                }
            }

            var repoResponse = await _repository.Authenticate(credential, password);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Login>();

            return repoResponse;
        }
    }
}
