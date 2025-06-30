using Simple.Ecommerce.App.Interfaces.Commands.LoginCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.LoginContracts;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.LoginCases.Commands
{
    public class CreateLoginCommand : ICreateLoginCommand
    {
        private readonly ILoginRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateLoginCommand(
            ILoginRepository repository, 
            IUserRepository userRepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _cacheHandler = cacheHandler;
            _useCache = useCache;
        }

        public async Task<Result<bool>> Execute(LoginRequest request)
        {
            var getLogin = await _repository.GetByCredential(request.Credential);
            if (getLogin.IsSuccess)
            {
                return Result<bool>.Failure(
                    new List<Error> { new("CreateLoginCommand.Conflict", "O login para essa credencial já existe!") }
                );
            }

            var getUser = await _userRepository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<bool>.Failure(getUser.Errors!);
            }

            var instance = new Login().Create(
                request.Id,
                request.UserId,
                request.Credential,
                request.Password,
                request.Type
            );
            if (instance.IsFailure)
            {
                return Result<bool>.Failure(instance.Errors!);
            }

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<bool>.Failure(createResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Login>();

            return Result<bool>.Success(true);
        }
    }
}
