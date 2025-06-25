using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class GetDiscountDTOsOrderQuery : IGetDiscountDTOsOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderDiscountRepository _orderDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountDTOsOrderQuery(
            IOrderRepository repository, 
            IOrderDiscountRepository orderDiscountRepository, 
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
            _orderDiscountRepository = orderDiscountRepository;
            _discountRepository = discountRepository;
            _discountTierRepository = discountTierRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _couponRepository = couponRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<OrderDiscountDTO>>> Execute(int orderId)
        {
            var getOrder = await GetAsync<Order>(
                () => _cacheHandler.GetFromCache<Order, Address, Order>(orderId, nameof(Address),
                    (cache, propName) => new Address().Create(
                        Convert.ToInt32(cache[$"{propName}_Number"]),
                        Convert.ToString(cache[$"{propName}_Street"])!,
                        Convert.ToString(cache[$"{propName}_Neighbourhood"])!,
                        Convert.ToString(cache[$"{propName}_City"])!,
                        Convert.ToString(cache[$"{propName}_Country"])!,
                        cache.GetNullableString($"{propName}_Complement"),
                        Convert.ToString(cache[$"{propName}_CEP"])!
                    ).GetValue(),
                    (cache, address) => new Order().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["UserId"]),
                        (OrderType)Convert.ToInt32(cache["OrderType"])!,
                        address,
                        cache.GetNullablePaymentMethod("PaymentMethod"),
                        cache.GetNullableDecimal("TotalPrice"),
                        cache.GetNullableDateTime("OrderDate"),
                        Convert.ToBoolean(cache["Confirmation"]),
                        Convert.ToString(cache["Status"])!
                    ).GetValue()),
                () => _repositoryHandler.GetFromRepository<Order>(
                    orderId,
                    async (id) => await _repository.Get(id)),
                () => _cacheHandler.SendToCache<Order>()
            );
            if (getOrder.IsFailure)
            {
                return Result<List<OrderDiscountDTO>>.Failure(getOrder.Errors!);
            }

            var order = getOrder.GetValue();

            var listOrderDiscount = await GetAsync<List<OrderDiscount>>(
                () => _cacheHandler.ListFromCacheByProperty<OrderDiscount, OrderDiscount>(nameof(OrderDiscount.OrderId), order.Id,
                    cache => new OrderDiscount().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["OrderId"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ).GetValue()),
                () => _repositoryHandler.ListFromRepository<OrderDiscount>(
                    order.Id,
                    async (filterId) => await _orderDiscountRepository.GetByOrderId(filterId)),
                () => _cacheHandler.SendToCache<OrderDiscount>()
            );
            if (listOrderDiscount.IsFailure)
            {
                return Result<List<OrderDiscountDTO>>.Failure(listOrderDiscount.Errors!);
            }

            List<int> discountIds = new();
            foreach (var orderDiscount in listOrderDiscount.GetValue())
            {
                discountIds.Add(orderDiscount.DiscountId);
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
                return Result<List<OrderDiscountDTO>>.Failure(listDiscount.Errors!);
            }

            List<OrderDiscountDTO> response = new();
            foreach (var (discount, orderDiscount) in listDiscount.GetValue().Zip(listOrderDiscount.GetValue()))
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
                        () => _repositoryHandler.ListFromRepository<DiscountTier, DiscountTierResponse>(
                            discount.Id,
                            async (filterId) => await _discountTierRepository.GetByDiscountId(filterId),
                            discountTier => new DiscountTierResponse(
                                discountTier.Id,
                                discountTier.MinQuantity,
                                discountTier.Value
                            )),
                        () => _cacheHandler.SendToCache<DiscountTier>()
                    );
                    if (tiersResponse.IsFailure)
                    {
                        return Result<List<OrderDiscountDTO>>.Failure(tiersResponse.Errors!);
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
                        return Result<List<OrderDiscountDTO>>.Failure(bundleItemsResponse.Errors!);
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
                        async (filterId) => await _couponRepository.GetByDiscountId(filterId),
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
                    return Result<List<OrderDiscountDTO>>.Failure(couponsResponse.Errors!);
                }

                response.Add( new OrderDiscountDTO(
                    orderDiscount.Id,
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

            return Result<List<OrderDiscountDTO>>.Success(response);
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
