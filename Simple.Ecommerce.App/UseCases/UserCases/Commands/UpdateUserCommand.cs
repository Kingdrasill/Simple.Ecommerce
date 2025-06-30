using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class UpdateUserCommand : IUpdateUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateUserCommand(
            IUserRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserResponse>> Execute(UserRequest request)
        {
            var getUser = await _repository.Get(request.Id);
            if (getUser.IsFailure)
            {
                return Result<UserResponse>.Failure(getUser.Errors!);
            }

            var instance = new User().Create(
                request.Id,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Password
            );
            if (instance.IsFailure)
            {
                return Result<UserResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<UserResponse>.Failure(updateResult.Errors!);
            }

            var user = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<User>();

            var response = new UserResponse(
                    user.Id,
                    user.Name,
                    user.Email,
                    user.PhoneNumber
                );

            return Result<UserResponse>.Success(response);
        }
    }
}
