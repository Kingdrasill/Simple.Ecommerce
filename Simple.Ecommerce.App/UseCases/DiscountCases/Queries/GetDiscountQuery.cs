using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetDiscountQuery : IGetDiscountQuery
    {
        private readonly IDiscountRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountQuery(
            IDiscountRepository repository,
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

        public async Task<Result<DiscountResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Discount, DiscountResponse>(id, cache =>
                    new DiscountResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        (DiscountType)Convert.ToInt32(cache["DiscountType"]),
                        (DiscountScope)Convert.ToInt32(cache["DiscountScope"]),
                        cache.GetNullableDiscountValueType("DiscountValueType"),
                        cache.GetNullableDecimal("Value"),
                        cache.GetNullableDateTime("ValidFrom"),
                        cache.GetNullableDateTime("ValidTo"),
                        Convert.ToBoolean(cache["IsActive"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Discount, DiscountResponse>(
                id,
                async (id) => await _repository.Get(id),
                discount => new DiscountResponse(
                    discount.Id,
                    discount.Name,
                    discount.DiscountType,
                    discount.DiscountScope,
                    discount.DiscountValueType,
                    discount.Value,
                    discount.ValidFrom,
                    discount.ValidTo,
                    discount.IsActive
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Discount>();

            return repoResponse;
        }
    }
}
