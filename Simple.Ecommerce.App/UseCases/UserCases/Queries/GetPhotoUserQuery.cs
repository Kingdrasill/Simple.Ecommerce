using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetPhotoUserQuery : IGetPhotoUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPhotoUserQuery(
            IUserRepository repository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _repositoryHandler = repositoryHandler;
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

            var repoResponse = await _repositoryHandler.GetFromRepository<User, UserPhotoResponse>(
                userId,
                async (id) => await _repository.Get(id),
                user => new UserPhotoResponse(
                    user.Id,
                    user.Photo is null ? null : new PhotoUserResponse(user.Photo.FileName)
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<User>();

            return repoResponse;
        }
    }
}
