using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class ListDiscountBundleItemDiscountQuery : IListDiscountBundleItemDiscountQuery
    {
        private readonly IDiscountBundleItemRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListDiscountBundleItemDiscountQuery(
            IDiscountBundleItemRepository repository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<DiscountBundleItemResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<DiscountBundleItem, DiscountBundleItemResponse>(cache =>
                    new DiscountBundleItemResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToInt32(cache["Quantity"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.ListFromRepository<DiscountBundleItem, DiscountBundleItemResponse>(
                async () => await _repository.List(),
                discountBundleItem => new DiscountBundleItemResponse(
                    discountBundleItem.Id,
                    discountBundleItem.ProductId,
                    discountBundleItem.Quantity,
                    discountBundleItem.DiscountId
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountBundleItem>();

            return repoResponse;
        }
    }
}
