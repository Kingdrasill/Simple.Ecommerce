using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetTierDiscountQuery : IGetTierDiscountQuery
    {
        private readonly IDiscountTierRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private UseCache _useCache;
        private ICacheHandler _cacheHandler;

        public GetTierDiscountQuery(
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

        public async Task<Result<DiscountTierResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<DiscountTier, DiscountTierResponse>(id, cache =>
                    new DiscountTierResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToInt32(cache["MinQuantity"]),
                        Convert.ToDecimal(cache["Value"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var reposResponse = await _repositoryHandler.GetFromRepository(
                id,
                async (id) => await _repository.Get(id),
                discountTier => new DiscountTierResponse(
                    discountTier.Id,
                    discountTier.Name,
                    discountTier.MinQuantity,
                    discountTier.Value,
                    discountTier.DiscountId
                ));
            if (reposResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountTier>();

            return reposResponse;
        }
    }
}
