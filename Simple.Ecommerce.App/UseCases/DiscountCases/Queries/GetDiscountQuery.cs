using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetDiscountQuery : IGetDiscountQuery
    {
        private readonly IDiscountRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountQuery(
            IDiscountRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
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

            var repoResponse = await GetFromRepository(id);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Discount>();

            return repoResponse;
        }

        private async Task<Result<DiscountResponse>> GetFromRepository(int id)
        {
            var getDiscount = await _repository.Get(id);
            if (getDiscount.IsFailure)
            {
                return Result<DiscountResponse>.Failure(getDiscount.Errors!);
            }

            var discount = getDiscount.GetValue();
            var response = new DiscountResponse(
                discount.Id,
                discount.Name,
                discount.DiscountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive
            );

            return Result<DiscountResponse>.Success(response);
        }
    }
}
