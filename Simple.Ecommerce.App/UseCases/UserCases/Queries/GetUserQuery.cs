using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetUserQuery : IGetUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetUserQuery(
            IUserRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserResponse>> Execute(int id, bool NoTracking = true)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<User, UserResponse>(id, cache =>
                    new UserResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToString(cache["Email"])!,
                        Convert.ToString(cache["PhoneNumber"])!
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository(id, NoTracking);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<User>();

            return repoResponse;
        }

        private async Task<Result<UserResponse>> GetFromRepository(int id, bool NoTracking)
        {
            var getResult = await _repository.Get(id, NoTracking);
            if (getResult.IsFailure)
            {
                return Result<UserResponse>.Failure(getResult.Errors!);
            }

            var user = getResult.GetValue();
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
