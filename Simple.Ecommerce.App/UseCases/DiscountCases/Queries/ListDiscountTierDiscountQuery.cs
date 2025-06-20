using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class ListDiscountTierDiscountQuery : IListDiscountTierDiscountQuery
    {
        private readonly IDiscountTierRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListDiscountTierDiscountQuery(
            IDiscountTierRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
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

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountTier>();

            return repoResponse;
        }

        private async Task<Result<List<DiscountTierResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<DiscountTierResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<DiscountTierResponse>();
            foreach (var discountTier in listResult.GetValue())
            {
                response.Add(new DiscountTierResponse(
                    discountTier.Id,
                    discountTier.MinQuantity,
                    discountTier.Value,
                    discountTier.DiscountId
                ));
            }

            return Result<List<DiscountTierResponse>>.Success(response);
        }
    }
}
