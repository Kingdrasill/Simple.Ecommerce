using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetPhotoUserQuery : IGetPhotoUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPhotoUserQuery(
            IUserRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserPhotoResponse>> Execute(int userId)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<User, PhotoUserResponse>(userId,
                    cache => {
                        if (cache.TryGetValue($"{nameof(Photo)}_FileName", out var fileName) && fileName is not null)
                            return new PhotoUserResponse(Convert.ToString(fileName)!);
                        return null;
                    });

                if (cacheResponse.IsSuccess)
                    return Result<UserPhotoResponse>.Success(
                        new UserPhotoResponse(
                            userId,
                            cacheResponse.GetValue()
                        ));
            }

            var repoResponse = await GetFromRepository(userId);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<User>();

            return repoResponse;
        }

        private async Task<Result<UserPhotoResponse>> GetFromRepository(int userId)
        {
            var userResult = await _repository.Get(userId);
            if (userResult.IsFailure)
            {
                return Result<UserPhotoResponse>.Failure(userResult.Errors!);
            }

            var response = new UserPhotoResponse(
                userId,
                userResult.GetValue().Photo is null ? null : new PhotoUserResponse(
                    userResult.GetValue().Photo!.FileName
                )
            );
            return Result<UserPhotoResponse>.Success(response);
        }
    }
}
