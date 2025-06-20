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
    public class ListCouponDiscountQuery : IListCouponDiscountQuery
    {
        private readonly ICouponRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListCouponDiscountQuery(
            ICouponRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<CouponResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Coupon, CouponResponse>(cache =>
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

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Coupon>();

            return repoResponse;
        }

        private async Task<Result<List<CouponResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<CouponResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<CouponResponse>();
            foreach (var coupon in listResult.GetValue())
            {
                response.Add(new CouponResponse(
                    coupon.Id,
                    coupon.Code,
                    coupon.IsUsed,
                    coupon.CreatedAt,
                    coupon.ExpirationAt,
                    coupon.UsedAt,
                    coupon.DiscountId
                ));
            }

            return Result<List<CouponResponse>>.Success(response);
        } 
    }
}
