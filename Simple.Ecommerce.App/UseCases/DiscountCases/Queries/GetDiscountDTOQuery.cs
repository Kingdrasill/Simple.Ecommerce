using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetDiscountDTOQuery : IGetDiscountDTOQuery
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountDTOQuery(
            IDiscountRepository repository,
            IDiscountTierRepository discountTierRepository,
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ICouponRepository couponRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountTierRepository = discountTierRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _couponRepository = couponRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountDTO>> Execute(int id)
        {
            var getDiscount = await GetAsync<Discount>(
                () => _cacheHandler.GetFromCache<Discount, Discount>(id, cache => 
                    new Discount().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        (DiscountType)Convert.ToInt32(cache["DiscountType"]),
                        (DiscountScope)Convert.ToInt32(cache["DiscountScope"]),
                        cache.GetNullableDiscountValueType("DiscountValueType"),
                        cache.GetNullableDecimal("Value"),
                        cache.GetNullableDateTime("ValidFrom"),
                        cache.GetNullableDateTime("ValidTo"),
                        cache.GetBoolean("IsActive")
                    ).GetValue()),
                () => GetFromRepositoryDiscount(id),
                () => _cacheHandler.SendToCache<Discount>()
            );
            if (getDiscount.IsFailure)
            {
                return Result<DiscountDTO>.Failure(getDiscount.Errors!);
            }

            var discount = getDiscount.GetValue();
            List<DiscountBundleItemResponse>? bundleItems = null;
            List<DiscountTierResponse>? tiers = null;

            if (discount.DiscountType == DiscountType.Tiered)
            {
                var tiersResponse = await GetAsync<List<DiscountTierResponse>>(
                    () => _cacheHandler.ListFromCacheByProperty<DiscountTier, DiscountTierResponse>(nameof(DiscountTier.DiscountId), discount.Id,
                        cache => new DiscountTierResponse(
                            Convert.ToInt32(cache["Id"]),
                            Convert.ToInt32(cache["MinQuality"]),
                            Convert.ToDecimal(cache["Value"])
                        )),
                    () => GetFromRepositoryDiscountTier(discount.Id),
                    () => _cacheHandler.SendToCache<DiscountTier>()
                );
                if (tiersResponse.IsFailure)
                {
                    return Result<DiscountDTO>.Failure(tiersResponse.Errors!);
                }
                tiers = tiersResponse.GetValue();
            }
            else if (discount.DiscountType == DiscountType.Bundle)
            {
                var bundleItemsResponse = await GetAsync<List<DiscountBundleItemResponse>>(
                    () => _cacheHandler.ListFromCacheByProperty<DiscountBundleItem, DiscountBundleItemResponse>(nameof(DiscountBundleItem.DiscountId), discount.Id,
                        cache => new DiscountBundleItemResponse(
                            Convert.ToInt32(cache["Id"]),
                            Convert.ToInt32(cache["ProductId"]),
                            Convert.ToInt32(cache["Quantity"])
                        )),
                    () => GetFromRepositoryDiscountBundleItem(discount.Id),
                    () => _cacheHandler.SendToCache<DiscountBundleItem>()
                );
                if (bundleItemsResponse.IsFailure)
                {
                    return Result<DiscountDTO>.Failure(bundleItemsResponse.Errors!);
                }
                bundleItems = bundleItemsResponse.GetValue();
            }

            var couponsResponse = await GetAsync<List<CouponResponse>>(
                () => _cacheHandler.ListFromCacheByProperty<Coupon, CouponResponse>(nameof(Coupon.DiscountId), discount.Id,
                    cache => new CouponResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Code"])!,
                        Convert.ToBoolean(cache["IsUsed"]),
                        Convert.ToDateTime(cache["CreatedAt"]),
                        Convert.ToDateTime(cache["ExpirationAt"]),
                        cache.GetNullableDateTime("UsedAt")
                    )),
                () => GetFromRepositoryCoupon(discount.Id),
                () => _cacheHandler.SendToCache<Coupon>()
            );
            if (couponsResponse.IsFailure)
            {
                return Result<DiscountDTO>.Failure(couponsResponse.Errors!);
            }

            var response = new DiscountDTO(
                discount.Id, 
                discount.Name,
                discount.DiscountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive,
                tiers,
                bundleItems,
                couponsResponse.GetValue()
            );

            return Result<DiscountDTO>.Success(response);
        }

        private async Task<Result<TResponse>> GetAsync<TResponse>(
            Func<Result<TResponse>> getFromCache,
            Func<Task<Result<TResponse>>> getFromRepo,
            Func<Task> sendToCache
        )
        {
            if (_useCache.Use)
            {
                var cacheResponse = getFromCache();
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await getFromRepo();
            if (repoResponse.IsSuccess && _useCache.Use)
                await sendToCache();

            return repoResponse;
        }

        private async Task<Result<Discount>> GetFromRepositoryDiscount(int discountId)
        {
            var getDiscount = await _repository.Get(discountId);
            return getDiscount;
        }

        private async Task<Result<List<DiscountBundleItemResponse>>> GetFromRepositoryDiscountBundleItem(int discountId)
        {
            var listBundleItem = await _discountBundleItemRepository.GetByDiscountId(discountId);
            if (listBundleItem.IsFailure)
            {
                return Result<List<DiscountBundleItemResponse>>.Failure(listBundleItem.Errors!);
            }

            var response = new List<DiscountBundleItemResponse>();
            foreach (var bundleItem in listBundleItem.GetValue())
            {
                response.Add(new DiscountBundleItemResponse(
                    bundleItem.Id,
                    bundleItem.ProductId,
                    bundleItem.Quantity
                ));
            }

            return Result<List<DiscountBundleItemResponse>>.Success(response);
        }

        private async Task<Result<List<DiscountTierResponse>>> GetFromRepositoryDiscountTier(int discountId)
        {
            var listTier = await _discountTierRepository.GetByDiscountId(discountId);
            if (listTier.IsFailure)
            {
                return Result<List<DiscountTierResponse>>.Failure(listTier.Errors!);
            }

            var response = new List<DiscountTierResponse>();
            foreach (var tier in listTier.GetValue())
            {
                response.Add(new DiscountTierResponse(
                    tier.Id,
                    tier.MinQuantity,
                    tier.Value
                ));
            }

            return Result<List<DiscountTierResponse>>.Success(response);
        }

        private async Task<Result<List<CouponResponse>>> GetFromRepositoryCoupon(int discountId)
        {
            var listCoupon = await _couponRepository.GetByDiscountId(discountId);
            if (listCoupon.IsFailure)
            { 
                return Result<List<CouponResponse>>.Failure(listCoupon.Errors!);
            }

            var response = new List<CouponResponse>();
            foreach (var coupon in listCoupon.GetValue())
            {
                response.Add(new CouponResponse(
                    coupon.Id,
                    coupon.Code,
                    coupon.IsUsed,
                    coupon.CreatedAt,
                    coupon.ExpirationAt,
                    coupon.UsedAt
                ));
            }

            return Result<List<CouponResponse>>.Success(response);
        }
    }
}
