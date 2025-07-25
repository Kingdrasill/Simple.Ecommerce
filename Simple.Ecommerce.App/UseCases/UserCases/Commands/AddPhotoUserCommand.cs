using ImageFile.Library.Core.Entities;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddPhotoUserCommand : IAddPhotoUserCommand
    {
        private readonly IAddPhotoUserUnitOfWork _addPhotoUserUoW;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddPhotoUserCommand(
            IAddPhotoUserUnitOfWork addPhotoUserUoW,
            IImageManager imageManager, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _addPhotoUserUoW = addPhotoUserUoW;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserPhotoResponse>> Execute(UserPhotoRequest request, Stream stream, string fileExtension)
        {
            FileImage? savedImage = null;
            await _addPhotoUserUoW.BeginTransaction();
            try
            {
                var getUser = await _addPhotoUserUoW.Users.Get(request.Id);
                if (getUser.IsFailure)
                {
                    throw new ResultException(getUser.Errors!);
                }

                var imageFile = await _imageManager.UploadImageAsync(
                    $"{Guid.NewGuid()}{fileExtension}",
                    stream,
                    request.Compress,
                    request.Deletable
                );
                if (imageFile.Result != ImageOperationResult.Success)
                {
                    throw new ResultException(new Error($"ImageFileSystem.{imageFile.Result}", imageFile.ErrorMessage!));
                }

                savedImage = imageFile.Image;

                var photo = new Photo(
                    imageFile.Image!.Name
                );
                var instance = getUser.GetValue();
                instance.AddOrUpdatePhoto(photo);
                if (instance.Validate() is { IsFailure: true } result)
                {
                    throw new ResultException(result.Errors!);
                }

                var updateResult = await _addPhotoUserUoW.Users.Update(instance, true);
                if (updateResult.IsFailure)
                {
                    throw new ResultException(updateResult.Errors!);
                }

                await _addPhotoUserUoW.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<User>();

                var user = updateResult.GetValue();

                var response = new UserPhotoResponse(
                    user.Id,
                    new PhotoUserResponse(user.Photo!.FileName)
                );

                return Result<UserPhotoResponse>.Success(response);
            }
            catch (ResultException rex)
            {
                await _addPhotoUserUoW.Rollback();
                if (savedImage is not null)
                    await _imageManager.DeleteImageAsync(savedImage.Name);
                return Result<UserPhotoResponse>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _addPhotoUserUoW.Rollback();
                if (savedImage is not null)
                    await _imageManager.DeleteImageAsync(savedImage.Name);
                return Result<UserPhotoResponse>.Failure(new List<Error> { new("AddPhotoUserCommand.Unknown", ex.Message) });
            }
        }
    }
}
