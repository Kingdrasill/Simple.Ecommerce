using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.ImageCleanup;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.Services.FileImage
{
    internal class UserImageCleanup : IImageCleanup
    {
        private readonly IUserRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UserImageCleanup(
            IUserRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public string RepositoryName => "User";

        public async Task<int> RemoveImages(List<string> imageNames)
        {
            var updatedCount = 0;

            var getUsers = await _repository.GetByImageNames(imageNames);
            if (getUsers.IsFailure)
            {
                return 0;
            }

            foreach (var user in getUsers.GetValue())
            {
                var deleteImageResult = await _repository.DeletePhotoFromUser(user.Id);
                if (deleteImageResult.IsSuccess)
                {
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<User>();

            return updatedCount;
        }
    }
}
