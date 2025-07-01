using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.PhotoContracts;
using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetPhotosProductQuery : IGetPhotosProductQuery
    {
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPhotosProductQuery(
            IProductPhotoRepository productPhotoRepository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _productPhotoRepository = productPhotoRepository;
            _repositoryHandler = repositoryHandler;
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

            var repoResponse = await _repositoryHandler.GetGroupedListFromRepository<ProductPhoto, PhotoProductResponse, ProductPhotosResponse>(
                productId,
                async (id) => await _productPhotoRepository.ListByProductId(id),
                photo => new PhotoProductResponse(
                    photo.Photo.FileName,
                    photo.Id
                ),
                (id, photos) => new ProductPhotosResponse(
                    id,
                    photos
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<ProductPhoto>();

            return repoResponse;
        }
    }
}
