using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetDiscountsProductQuery : IGetDiscountsProductQuery
    {
        private readonly IProductRepository _repository;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountsProductQuery(
            IProductRepository repository, 
            IProductDiscountRepository productDiscountRepository, 
            IDiscountRepository discountRepository, 
            IDiscountTierRepository discountTierRepository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ICouponRepository couponRepository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productDiscountRepository = productDiscountRepository;
            _discountRepository = discountRepository;
            _discountTierRepository = discountTierRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _couponRepository = couponRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<ProductDiscountDTO>>> Execute(int productId)
        {
            var getProduct = await GetAsync<Product>(
                () => _cacheHandler.GetFromCache<Product, Product>(productId,
                    cache => new Product().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToDecimal(cache["Price"]),
                        Convert.ToString(cache["Description"])!,
                        Convert.ToInt32(cache["Stock"])
                    ).GetValue()),
                () => _repositoryHandler.GetFromRepository<Product>(
                    productId,
                    async (id) => await _repository.Get(id)),
                () => _cacheHandler.SendToCache<Product>()
            );
            if (getProduct.IsFailure)
            {
                return Result<List<ProductDiscountDTO>>.Failure(getProduct.Errors!);
            }

            var product = getProduct.GetValue();

            var listProductDiscount = await GetAsync<List<ProductDiscount>>(
                () => _cacheHandler.ListFromCacheByProperty<ProductDiscount, ProductDiscount>(nameof(ProductDiscount.ProductId), productId,
                    cache => new ProductDiscount().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ).GetValue()),
                () => _repositoryHandler.ListFromRepository<ProductDiscount>(
                    productId,
                    async (filterId) => await _productDiscountRepository.GetByProductId(productId)),
                () => _cacheHandler.SendToCache<ProductDiscount>()
            );
            if (listProductDiscount.IsFailure)
            {
                return Result<List<ProductDiscountDTO>>.Failure(listProductDiscount.Errors!);
            }

            List<int> discountIds = new();
            foreach (var productDiscount in listProductDiscount.GetValue())
            {
                discountIds.Add(productDiscount.DiscountId);
            }

            var listDiscount = await GetAsync<List<Discount>>(
                () => _cacheHandler.ListFromCacheByPropertyIn<Discount, Discount>(nameof(Discount.Id), discountIds.Cast<object>(),
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
                () => _repositoryHandler.GetFromRepositoryFromIds<Discount, int>(
                    discountIds,
                    async (discountId) => await _discountRepository.Get(discountId)),
                () => _cacheHandler.SendToCache<Discount>()
            );
            if (listDiscount.IsFailure)
            {
                return Result<List<ProductDiscountDTO>>.Failure(listDiscount.Errors!);
            }

            List<ProductDiscountDTO> response = new();
            foreach (var (discount, productDiscount) in listDiscount.GetValue().Zip(listProductDiscount.GetValue()))
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
                            discountTier => new DiscountTierResponse(
                                discountTier.Id,
                                discountTier.Name,
                                discountTier.MinQuantity,
                                discountTier.Value
                            )),
                        () => _cacheHandler.SendToCache<DiscountTier>()
                    );
                    if (tiersResponse.IsFailure)
                    {
                        return Result<List<ProductDiscountDTO>>.Failure(tiersResponse.Errors!);
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
                            discountBundleItem => new DiscountBundleItemResponse(
                                discountBundleItem.Id,
                                discountBundleItem.ProductId,
                                discountBundleItem.Quantity
                            )),
                        () => _cacheHandler.SendToCache<DiscountBundleItem>()
                    );
                    if (bundleItemsResponse.IsFailure)
                    {
                        return Result<List<ProductDiscountDTO>>.Failure(bundleItemsResponse.Errors!);
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
                    return Result<List<ProductDiscountDTO>>.Failure(couponsResponse.Errors!);
                }

                response.Add(new ProductDiscountDTO(
                    productDiscount.Id,
                    new DiscountCompleteDTO(
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
                    )
                ));
            }

            return Result<List<ProductDiscountDTO>>.Success(response);
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
