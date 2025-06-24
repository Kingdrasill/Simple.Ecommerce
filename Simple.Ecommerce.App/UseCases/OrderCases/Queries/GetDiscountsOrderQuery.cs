using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
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
    public class GetDiscountsOrderQuery : IGetDiscountsOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderDiscountRepository _orderDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountsOrderQuery(
            IOrderRepository repository, 
            IOrderDiscountRepository orderDiscountRepository, 
            IDiscountRepository discountRepository, 
            IDiscountTierRepository discountTierRepository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ICouponRepository couponRepository, 
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
                () => GetFromRepositoryOrder(orderId),
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
                () => GetFromRepositoryOrderDiscount(order.Id),
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
                () => GetFromRepositoryDiscount(discountIds),
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
                        () => GetFromRepositoryDiscountTier(discount.Id),
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
                        () => GetFromRepositoryDiscountBundleItem(discount.Id),
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
                    () => GetFromRepositoryCoupon(discount.Id),
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

        private async Task<Result<Order>> GetFromRepositoryOrder(int id)
        {
            var getOrder = await _repository.Get(id);
            return getOrder;
        }

        private async Task<Result<List<OrderDiscount>>> GetFromRepositoryOrderDiscount(int orderId)
        {
            var listOrderDiscount = await _orderDiscountRepository.GetByOrderId(orderId);
            return listOrderDiscount;
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
