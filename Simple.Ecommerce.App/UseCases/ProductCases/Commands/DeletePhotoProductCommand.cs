using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class DeletePhotoProductCommand : IDeletePhotoProductCommand
    {
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeletePhotoProductCommand(
            IProductPhotoRepository productPhotoRepository, 
            IImageManager imageManager,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _productPhotoRepository = productPhotoRepository;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int productPhotoId)
        {
            var getProductPhoto = await _productPhotoRepository.Get(productPhotoId);
            if (getProductPhoto.IsFailure)
            {
                return Result<bool>.Failure(getProductPhoto.Errors!);
            }

            var productPhoto = getProductPhoto.GetValue();

            var deleteImage = await _imageManager.DeleteImageAsync(productPhoto.Photo.FileName);
            if (deleteImage.Result != ImageOperationResult.Success)
            {
                return Result<bool>.Failure(new List<Error> { new($"ImageFileSystem.{deleteImage.Result}", deleteImage.ErrorMessage!) });
            }

            var deleteResult = await _productPhotoRepository.Delete(productPhotoId);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<ProductPhoto>();

            return deleteResult;
        }
    }
}
