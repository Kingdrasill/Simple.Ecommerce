using ImageFile.Library.Core.Entities;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class AddPhotoProductCommand : IAddPhotoProductCommand
    {
        private readonly IAddPhotoProductUnitOfWork _addPhotoProductUoW;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddPhotoProductCommand(
            IAddPhotoProductUnitOfWork addPhotoProductUoW,
            IImageManager imageManager,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _addPhotoProductUoW = addPhotoProductUoW;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductPhotoResponse>> Execute(ProductPhotoRequest request, Stream stream, string fileExtension)
        {
            FileImage? savedImage = null;
            await _addPhotoProductUoW.BeginTransaction();
            try
            {
                var getProduct = await _addPhotoProductUoW.Products.Get(request.ProductId);
                if (getProduct.IsFailure)
                {
                    throw new ResultException(getProduct.Errors!);
                }

                var imageFile = await _imageManager.UploadImageAsync(
                    $"{Guid.NewGuid()}{fileExtension}",
                    stream,
                    request.Compress,
                    request.Deletable
                );
                if (imageFile.Result != ImageOperationResult.Success)
                {
                    throw new ResultException(new Error($"AddPhotoProductCommand.ImageFileSystem.{imageFile.Result}", imageFile.ErrorMessage!));
                }

                savedImage = imageFile.Image;

                var photo = new Photo().Create(
                    imageFile.Image!.Name
                );
                if (photo.IsFailure)
                {
                    throw new ResultException(photo.Errors!);
                }

                var instance = new ProductPhoto().Create(
                    0,
                    request.ProductId,
                    photo.GetValue()
                );
                if (instance.IsFailure)
                {
                    throw new ResultException(instance.Errors!);
                }

                var createResult = await _addPhotoProductUoW.ProductPhotos.Create(instance.GetValue(), true);
                if (createResult.IsFailure)
                {
                    throw new ResultException(createResult.Errors!);
                }

                await _addPhotoProductUoW.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<ProductPhoto>();

                var productPhoto = createResult.GetValue();

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<ProductPhoto>();

                var photoResponse = new PhotoProductResponse(
                    productPhoto.Photo.FileName,
                    productPhoto.Id
                );

                var response = new ProductPhotoResponse(
                    photoResponse,
                    productPhoto.ProductId
                );

                return Result<ProductPhotoResponse>.Success(response);
            }
            catch (ResultException rex)
            {
                await _addPhotoProductUoW.Rollback();
                if (savedImage is not null)
                    await _imageManager.DeleteImageAsync(savedImage.Name);
                return Result<ProductPhotoResponse>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _addPhotoProductUoW.Rollback();
                if (savedImage is not null)
                    await _imageManager.DeleteImageAsync(savedImage.Name);
                return Result<ProductPhotoResponse>.Failure(new List<Error> { new("AddPhotoProductCommand.Unknown", ex.Message) });
            }
        }
    }
}
