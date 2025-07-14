using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemovePhotoUserCommand : IRemovePhotoUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePhotoUserCommand(
            IUserRepository repository, 
            ISaverTransectioner unityOfWork,
            IImageManager imageManager, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int userId)
        {            
            var getUser = await _repository.Get(userId);
            if (getUser.IsFailure)
            {
                return Result<bool>.Failure(getUser.Errors!);
            }

            var user = getUser.GetValue();
            if (user.Photo is null)
            {
                return Result<bool>.Success(true);
            }

            var deletePhotoResult = await _repository.DeletePhotoFromUser(user.Id);
            if (deletePhotoResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<User>();

                await _imageManager.DeleteImageAsync(user.Photo.FileName);
            }

            return deletePhotoResult;
        }
    }
}
