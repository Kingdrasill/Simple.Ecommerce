using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemovePhotoUserCommand : IRemovePhotoUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePhotoUserCommand(
            IUserRepository repository, 
            IImageManager imageManager, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
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

            var deleteImage = await _imageManager.DeleteImageAsync(user.Photo.FileName);
            if (deleteImage.Result != ImageOperationResult.Success)
            {
                return Result<bool>.Failure(new List<Error> { new($"ImageFileSystem.{deleteImage.Result}", deleteImage.ErrorMessage!) });
            }

            var deletePhotoResult = await _repository.DeletePhotoFromUser(user.Id);
            
            if (deletePhotoResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<User>();

            return deletePhotoResult;
        }
    }
}
