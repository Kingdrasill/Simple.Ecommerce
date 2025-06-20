using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetDiscountTierDiscountQuery : IGetDiscountTierDiscountQuery
    {
        private readonly IDiscountTierRepository _repository;
        private UseCache _useCache;
        private ICacheHandler _cacheHandler;

        public GetDiscountTierDiscountQuery(
            IDiscountTierRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
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
                        Convert.ToInt32(cache["MinQuantity"]),
                        Convert.ToDecimal(cache["Value"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var reposResponse = await GetFromRepository(id);
            if (reposResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountTier>();

            return reposResponse;
        }

        private async Task<Result<DiscountTierResponse>> GetFromRepository(int id)
        {
            var getResult = await _repository.Get(id);
            if (getResult.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(getResult.Errors!);
            }

            var discountTier = getResult.GetValue();
            var response = new DiscountTierResponse(
                discountTier.Id,
                discountTier.MinQuantity,
                discountTier.Value,
                discountTier.DiscountId
            );

            return Result<DiscountTierResponse>.Success(response);
        }
    }
}
