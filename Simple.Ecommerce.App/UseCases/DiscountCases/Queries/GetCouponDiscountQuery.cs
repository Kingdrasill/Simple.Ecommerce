using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetCouponDiscountQuery : IGetCouponDiscountQuery
    {
        private readonly ICouponRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCouponDiscountQuery(
            ICouponRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<CouponResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Coupon, CouponResponse>(id, cache =>
                    new CouponResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Code"])!,
                        Convert.ToBoolean(cache["IsUsed"]),
                        Convert.ToDateTime(cache["CreatedAt"]),
                        Convert.ToDateTime(cache["ExpirationAt"]),
                        cache.GetNullableDateTime("UsedAt"),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository(id);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Coupon>();

            return repoResponse;
        }

        private async Task<Result<CouponResponse>> GetFromRepository(int id)
        {
            var getCoupon = await _repository.Get(id);
            if (getCoupon.IsFailure)
            {
                return Result<CouponResponse>.Failure(getCoupon.Errors!);
            }

            var coupon = getCoupon.GetValue();
            var response = new CouponResponse(
                coupon.Id,
                coupon.Code,
                coupon.IsUsed,
                coupon.CreatedAt,
                coupon.ExpirationAt,
                coupon.UsedAt,
                coupon.DiscountId
            );

            return Result<CouponResponse>.Success(response);
        }
    }
}
