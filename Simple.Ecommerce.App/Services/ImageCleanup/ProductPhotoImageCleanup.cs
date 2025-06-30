using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.ImageCleanup;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.Services.FileImage
{
    internal class ProductPhotoImageCleanup : IImageCleanup
    {
        private readonly IProductPhotoRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ProductPhotoImageCleanup(
            IProductPhotoRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public string RepositoryName => "ProductPhotos";

        public async Task<int> RemoveImages(List<string> imageNames)
        {
            var updatedCount = 0;

            var getProductPhotos = await _repository.GetByImageNames(imageNames);
            if (getProductPhotos.IsFailure)
            {
                return 0;
            }

            foreach (var productPhoto in getProductPhotos.GetValue())
            {
                var deleteImageResult = await _repository.Delete(productPhoto.Id);
                if (deleteImageResult.IsSuccess)
                {
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<ProductPhoto>();

            return updatedCount;
        }
    }
}
