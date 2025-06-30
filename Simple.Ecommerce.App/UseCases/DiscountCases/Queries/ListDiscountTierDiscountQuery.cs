using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class ListDiscountTierDiscountQuery : IListDiscountTierDiscountQuery
    {
        private readonly IDiscountTierRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListDiscountTierDiscountQuery(
            IDiscountTierRepository repository,
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

        public async Task<Result<List<DiscountTierResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<DiscountTier, DiscountTierResponse>(cache =>
                    new DiscountTierResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["MinQuantity"]),
                        Convert.ToDecimal(cache["Value"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.ListFromRepository<DiscountTier, DiscountTierResponse>(
                async () => await _repository.List(),
                discountTier => new DiscountTierResponse(
                    discountTier.Id,
                    discountTier.MinQuantity,
                    discountTier.Value,
                    discountTier.DiscountId
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountTier>();

            return repoResponse;
        }
    }
}
