using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class ListDiscountDTOQuery : IListDiscountDTOQuery
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListDiscountDTOQuery(
            IDiscountRepository repository, 
            IDiscountTierRepository discountTierRepository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ICouponRepository couponRepository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountTierRepository = discountTierRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _couponRepository = couponRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<DiscountCompleteDTO>>> Execute()
        {
            var listDiscount = await GetAsync<List<Discount>>(
                () => _cacheHandler.ListFromCache<Discount, Discount>(
                    cache => new Discount().Create(
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
                () => _repositoryHandler.ListFromRepository<Discount>(
                    async () => await _repository.List()),
                () => _cacheHandler.SendToCache<Discount>()
            );
            if (listDiscount.IsFailure)
            {
                return Result<List<DiscountCompleteDTO>>.Failure(listDiscount.Errors!);
            }

            List<DiscountCompleteDTO> response = new();
            foreach (var discount in listDiscount.GetValue())
            {
                List<DiscountBundleItemResponse>? bundleItems = null;
                List<DiscountTierResponse>? tiers = null;

                if (discount.DiscountType == DiscountType.Tiered)
                {
                    var tiersResponse = await GetAsync<List<DiscountTierResponse>>(
                        () => _cacheHandler.ListFromCacheByProperty<DiscountTier, DiscountTierResponse>(nameof(DiscountTier.DiscountId), discount.Id,
                            cache => new DiscountTierResponse(
                                Convert.ToInt32(cache["Id"]),
                                Convert.ToString(cache["Name"])!,
                                Convert.ToInt32(cache["MinQuality"]),
                                Convert.ToDecimal(cache["Value"])
                            )),
                        () => _repositoryHandler.ListFromRepository<DiscountTier, DiscountTierResponse>(
                            discount.Id,
                            async (filterId) => await _discountTierRepository.GetByDiscountId(filterId),
                            tier => new DiscountTierResponse(
                                tier.Id,
                                tier.Name,
                                tier.MinQuantity,
                                tier.Value
                            )),
                        () => _cacheHandler.SendToCache<DiscountTier>()
                    );
                    if (tiersResponse.IsFailure)
                    {
                        return Result<List<DiscountCompleteDTO>>.Failure(tiersResponse.Errors!);
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
                        () => _repositoryHandler.ListFromRepository<DiscountBundleItem, DiscountBundleItemResponse>(
                            discount.Id,
                            async (filterId) => await _discountBundleItemRepository.GetByDiscountId(filterId),
                            bundleItem => new DiscountBundleItemResponse(
                                bundleItem.Id,
                                bundleItem.ProductId,
                                bundleItem.Quantity
                            )),
                        () => _cacheHandler.SendToCache<DiscountBundleItem>()
                    );
                    if (bundleItemsResponse.IsFailure)
                    {
                        return Result<List<DiscountCompleteDTO>>.Failure(bundleItemsResponse.Errors!);
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
                    () => _repositoryHandler.ListFromRepository<Coupon, CouponResponse>(
                        discount.Id,
                        async (filterId) => await _couponRepository.ListByDiscountId(filterId),
                        coupon => new CouponResponse(
                            coupon.Id,
                            coupon.Code,
                            coupon.IsUsed,
                            coupon.CreatedAt,
                            coupon.ExpirationAt,
                            coupon.UsedAt
                        )),
                    () => _cacheHandler.SendToCache<Coupon>()
                );
                if (couponsResponse.IsFailure)
                {
                    return Result<List<DiscountCompleteDTO>>.Failure(couponsResponse.Errors!);
                }

                response.Add(new DiscountCompleteDTO(
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
                ));
            }

            return Result<List<DiscountCompleteDTO>>.Success(response);
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
    }
}
