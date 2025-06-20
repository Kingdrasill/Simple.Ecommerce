using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;

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
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountsProductQuery(
            IProductRepository repository, 
            IProductDiscountRepository productDiscountRepository, 
            IDiscountRepository discountRepository, 
            IDiscountTierRepository discountTierRepository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ICouponRepository couponRepository, 
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
                () => GetFromRepositoryProduct(productId),
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
                () => GetFromRepositoryProductDiscount(productId),
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
                () => GetFromRepositoryDiscount(discountIds),
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
                                Convert.ToInt32(cache["MinQuality"]),
                                Convert.ToDecimal(cache["Value"])
                            )),
                        () => GetFromRepositoryDiscountTier(discount.Id),
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
                        () => GetFromRepositoryDiscountBundleItem(discount.Id),
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
                    () => GetFromRepositoryCoupon(discount.Id),
                    () => _cacheHandler.SendToCache<Coupon>()
                );
                if (couponsResponse.IsFailure)
                {
                    return Result<List<ProductDiscountDTO>>.Failure(couponsResponse.Errors!);
                }

                response.Add(new ProductDiscountDTO(
                    productDiscount.Id,
                    new DiscountDTO(
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

        private async Task<Result<Product>> GetFromRepositoryProduct(int id)
        {
            var getProduct = await _repository.Get(id);
            return getProduct;
        }

        private async Task<Result<List<ProductDiscount>>> GetFromRepositoryProductDiscount(int productId)
        {
            var listProductDiscount = await _productDiscountRepository.GetByProductId(productId);
            return listProductDiscount;
        }

        private async Task<Result<List<Discount>>> GetFromRepositoryDiscount(List<int> discountIds)
        {
            var listDiscount = new List<Discount>();
            foreach (var discountId in discountIds)
            {
                var discount = await _discountRepository.Get(discountId);
                if (discount.IsFailure)
                {
                    return Result<List<Discount>>.Failure(discount.Errors!);
                }
                listDiscount.Add(discount.GetValue());
            }

            return Result<List<Discount>>.Success(listDiscount);
        }

        private async Task<Result<List<DiscountTierResponse>>> GetFromRepositoryDiscountTier(int discountId)
        {
            var listDiscountTier = await _discountTierRepository.GetByDiscountId(discountId);
            if (listDiscountTier.IsFailure)
            {
                return Result<List<DiscountTierResponse>>.Failure(listDiscountTier.Errors!);
            }

            var response = new List<DiscountTierResponse>();
            foreach (var discountTier in listDiscountTier.GetValue())
            {
                response.Add(new DiscountTierResponse(
                    discountTier.Id,
                    discountTier.MinQuantity,
                    discountTier.Value
                ));
            }

            return Result<List<DiscountTierResponse>>.Success(response);
        }

        private async Task<Result<List<DiscountBundleItemResponse>>> GetFromRepositoryDiscountBundleItem(int discountId)
        {
            var listBundleItem = await _discountBundleItemRepository.GetByDiscountId(discountId);
            if (listBundleItem.IsFailure)
            {
                return Result<List<DiscountBundleItemResponse>>.Failure(listBundleItem.Errors!);
            }

            var response = new List<DiscountBundleItemResponse>();
            foreach (var discountBundleItem in listBundleItem.GetValue())
            {
                response.Add(new DiscountBundleItemResponse(
                    discountBundleItem.Id,
                    discountBundleItem.ProductId,
                    discountBundleItem.Quantity
                ));
            }

            return Result<List<DiscountBundleItemResponse>>.Success(response);
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
