using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetPhotosProductQuery : IGetPhotosProductQuery
    {
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPhotosProductQuery(
            IProductPhotoRepository productPhotoRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _productPhotoRepository = productPhotoRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductPhotosResponse>> Execute(int productId)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCacheByProperty<ProductPhoto, PhotoProductResponse>(nameof(ProductPhoto.ProductId), productId, 
                    cache => new PhotoProductResponse(
                        Convert.ToString(cache[$"{nameof(Photo)}_FileName"])!,
                        Convert.ToInt32(cache["Id"])
                    ));
                if (cacheResponse.IsSuccess)
                    return Result<ProductPhotosResponse>.Success(new ProductPhotosResponse(
                        productId,
                        cacheResponse.GetValue()
                    ));
            }

            var repoResponse = await GetFromRepository(productId);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<ProductPhoto>();

            return repoResponse;
        }

        private async Task<Result<ProductPhotosResponse>> GetFromRepository(int productId)
        {
            var listResult = await _productPhotoRepository.ListByProductId(productId);
            if (listResult.IsFailure)
            {
                return Result<ProductPhotosResponse>.Failure(listResult.Errors!);
            }

            var response = new List<PhotoProductResponse>();
            foreach (var photo in listResult.GetValue())
            {
                response.Add(new PhotoProductResponse(
                    photo.Photo.FileName,
                    photo.Id
                ));
            }

            return Result<ProductPhotosResponse>.Success(new ProductPhotosResponse(
                productId,
                response
            ));
        }
    }
}
