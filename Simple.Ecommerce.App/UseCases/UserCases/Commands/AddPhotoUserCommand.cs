using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddPhotoUserCommand : IAddPhotoUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddPhotoUserCommand(
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

        public async Task<Result<UserPhotoResponse>> Execute(UserPhotoRequest request, Stream stream, string fileExtension)
        {
            var getUser = await _repository.Get(request.Id);
            if (getUser.IsFailure)
            {
                return Result<UserPhotoResponse>.Failure(getUser.Errors!);
            }

            var imageFile = await _imageManager.UploadImageAsync(
                $"{Guid.NewGuid()}{fileExtension}",
                stream,
                request.Compress,
                request.Deletable
            );
            if (imageFile.Result != ImageOperationResult.Success)
            {
                return Result<UserPhotoResponse>.Failure(new List<Error> { new($"ImageFileSystem.{imageFile.Result}", imageFile.ErrorMessage!) });
            }

            var photo = new Photo().Create(
                imageFile.Image!.Name
            );
            if (photo.IsFailure) 
            {
                return Result<UserPhotoResponse>.Failure(photo.Errors!);
            }

            var instance = getUser.GetValue();
            instance.AddOrUpdatePhoto(photo.GetValue());

            var updateResult = await _repository.Update(instance);
            if (updateResult.IsFailure) 
            {
                return Result<UserPhotoResponse>.Failure(updateResult.Errors!);
            }

            var user = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<User>();

            var response = new UserPhotoResponse(
                user.Id,
                new PhotoResponse(
                    user.Photo!.FileName
                )
            );

            return Result<UserPhotoResponse>.Success(response);
        }
    }
}
