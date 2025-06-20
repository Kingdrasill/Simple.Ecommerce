using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class AddPhotoProductCommand : IAddPhotoProductCommand
    {
        private readonly IProductPhotoRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddPhotoProductCommand(
            IProductPhotoRepository repository, 
            IProductRepository productRepository, 
            IImageManager imageManager,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productRepository = productRepository;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductPhotoResponse>> Execute(ProductPhotoRequest request, Stream stream, string fileExtension)
        {
            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<ProductPhotoResponse>.Failure(getProduct.Errors!);
            }

            var imageFile = await _imageManager.UploadImageAsync(
                $"{Guid.NewGuid()}{fileExtension}",
                stream,
                request.Compress,
                request.Deletable
            );
            if (imageFile.Result != ImageOperationResult.Success)
            {
                return Result<ProductPhotoResponse>.Failure(new List<Error> { new($"ImageFileSystem.{imageFile.Result}", imageFile.ErrorMessage!) });
            }

            var photo = new Photo().Create(
                imageFile.Image!.Name    
            );
            if (photo.IsFailure)
            {
                return Result<ProductPhotoResponse>.Failure(photo.Errors!);
            }

            var instance = new ProductPhoto().Create(
                0,
                request.ProductId,
                photo.GetValue()
            );
            if (instance.IsFailure)
            {
                return Result<ProductPhotoResponse>.Failure(instance.Errors!);
            }

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<ProductPhotoResponse>.Failure(createResult.Errors!);
            }

            var productPhoto = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<ProductPhoto>();

            var photoResponse = new PhotoResponse(
                productPhoto.Photo.FileName,
                productPhoto.Id
            );

            var response = new ProductPhotoResponse(
                photoResponse,
                productPhoto.ProductId
            );

            return Result<ProductPhotoResponse>.Success(response);
        }
    }
}
