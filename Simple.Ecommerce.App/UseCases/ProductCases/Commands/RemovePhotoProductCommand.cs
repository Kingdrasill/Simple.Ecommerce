using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class RemovePhotoProductCommand : IRemovePhotoProductCommand
    {
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePhotoProductCommand(
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

            var deleteResult = await _productPhotoRepository.Delete(productPhotoId);
            if (deleteResult.IsFailure)
            {
                return Result<bool>.Failure(deleteResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<ProductPhoto>();

            var deleteImage = await _imageManager.DeleteImageAsync(productPhoto.Photo.FileName);
            if (deleteImage.Result != ImageOperationResult.Success)
            {
                List<Error> errors = new List<Error>{ new("RemovePhotoProductCommand.DeleteError", "A imagem foi deletada do produto, porém houve um erro no sistema de arquivos e será apagada depois.") };
                errors.Add(new($"ImageFileSystem.{deleteImage.Result}", deleteImage.ErrorMessage!));
                return Result<bool>.Failure(errors);
            }

            return deleteResult;
        }
    }
}
