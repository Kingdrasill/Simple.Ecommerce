using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetCouponDiscountQuery : IGetCouponDiscountQuery
    {
        private readonly ICouponRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCouponDiscountQuery(
            ICouponRepository repository, 
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

            var repoResponse = await _repositoryHandler.GetFromRepository<Coupon, CouponResponse>(
                id,
                async (id) => await _repository.Get(id),
                coupon => new CouponResponse(
                    coupon.Id,
                    coupon.Code,
                    coupon.IsUsed,
                    coupon.CreatedAt,
                    coupon.ExpirationAt,
                    coupon.UsedAt,
                    coupon.DiscountId
                )
            );
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Coupon>();

            return repoResponse;
        }
    }
}
