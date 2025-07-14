using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class GetCompleteOrderQuery : IGetCompleteOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCompleteOrderQuery(
            IOrderRepository repository, 
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

        public async Task<Result<OrderCompleteDTO>> Execute(int id)
        {
            string failed = "";
            if (_useCache.Use)
            {
                var cacheResponse = GetCompleteOrderFromCache(id, out failed);
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repository.GetCompleteOrder(id);
            if (repoResponse.IsSuccess)
            {
                if (!repoResponse.GetValue().Confirmation)
                {
                    return Result<OrderCompleteDTO>.Failure(new List<Error> { new("GetCompleteOrderQuery.NotConfirmed", "Esse pedido ainda não foi confirmado!") });
                }

                if (_useCache.Use)
                {
                    if (failed.Equals(nameof(Order)))
                        await _cacheHandler.SendToCache<Order>();
                    if (failed.Equals(nameof(OrderItem)))
                        await _cacheHandler.SendToCache<OrderItem>();
                    if (failed.Equals(nameof(User)))
                        await _cacheHandler.SendToCache<User>();
                    if (failed.Equals(nameof(Product)))
                        await _cacheHandler.SendToCache<Product>();
                    if (failed.Equals(nameof(Discount)))
                        await _cacheHandler.SendToCache<Discount>();
                    if (failed.Equals(nameof(DiscountTier)))
                        await _cacheHandler.SendToCache<DiscountTier>();
                }
            }

            return repoResponse;
        }

        private Result<OrderCompleteDTO> GetCompleteOrderFromCache(int orderId, out string failed)
        {
            var orderResponse = GetOrderFromcCache(orderId);
            if (orderResponse.IsFailure)
            {
                failed = nameof(Order);
                return Result<OrderCompleteDTO>.Failure(new());
            }
            var order = orderResponse.GetValue();
            if (!order.Confirmation)
            {
                failed = nameof(Order);
                return Result<OrderCompleteDTO>.Failure(new());
            }

            var userResponse = GetUserFromCache(order.UserId);
            if (userResponse.IsFailure)
            {
                failed = nameof(User);
                return Result<OrderCompleteDTO>.Failure(new());
            }
            var user = userResponse.GetValue();

            var orderItemsResponse = GetOrderItemsFromCache(orderId);
            if (orderItemsResponse.IsFailure)
            {
                failed = nameof(OrderItem);
                return Result<OrderCompleteDTO>.Failure(new());
            }
            var orderItems = orderItemsResponse.GetValue();

            var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();
            var discountIds = orderItems.Where(oi => oi.DiscountId.HasValue).Select(oi => oi.DiscountId).Distinct().ToList();

            var productsResponse = GetProductsFromCache(productIds);
            if (productsResponse.IsFailure)
            {
                failed = nameof(Product);
                return Result<OrderCompleteDTO>.Failure(new());
            }
            var productsMap = productsResponse.GetValue().ToDictionary(p => p.Id);

            var discountsResponse = GetOrderItemDiscountsFromCache(discountIds);
            if (discountsResponse.IsFailure)
            {
                failed = nameof(Discount);
                return Result<OrderCompleteDTO>.Failure(new());
            }
            var discounts = discountsResponse.GetValue();
            var discountsMap = discounts.ToDictionary(p => p.Id);

            var tierDiscountIds = discounts
                .Where(d => d.DiscountType == DiscountType.Tiered)
                .Select(d => d.Id)
                .ToList();

            var allTiersDiscount = new List<DiscountTier>();
            foreach (var discountId in tierDiscountIds)
            {
                var tiersDiscountResponse = GetTiersFromCache(discountId);
                if (!tiersDiscountResponse.IsFailure)
                {
                    failed = nameof(DiscountTier);
                    return Result<OrderCompleteDTO>.Failure(new());
                }
                allTiersDiscount.AddRange(tiersDiscountResponse.GetValue());
            }

            var groupedTiers = allTiersDiscount.GroupBy(dt => dt.DiscountId)
                .ToDictionary(g => g.Key, g => g.Select(dt => new DiscountTierResponse(
                    dt.Id,
                    dt.Name,
                    dt.MinQuantity,
                    dt.Value,
                    null
                )));

            DiscountItemDTO? orderAppliedDiscount = null;
            if (order.DiscountId is not null)
            {
                var orderDiscountResponse = GetOrderDiscountFromCache(order.DiscountId.Value);
                if (orderDiscountResponse.IsFailure)
                {
                    failed = nameof(Discount);
                    return Result<OrderCompleteDTO>.Failure(new());
                }
                var orderDiscount = orderDiscountResponse.GetValue();

                orderAppliedDiscount = new DiscountItemDTO(
                    orderDiscount.Id,
                    orderDiscount.Name,
                    orderDiscount.DiscountType,
                    orderDiscount.DiscountScope,
                    orderDiscount.DiscountValueType,
                    orderDiscount.Value,
                    orderDiscount.ValidFrom,
                    orderDiscount.ValidTo,
                    orderDiscount.IsActive,
                    null
                );
            }

            var items = new List<OrderItemDTO>();
            var bundledItems = new List<BundleItemsDTO>();

            foreach (var item in orderItems)
            {
                var productName = productsMap.TryGetValue(item.ProductId, out var prod) ? prod.Name : "Unknown Product";
                DiscountItemDTO? discountItemDTO = null;
                Discount discount = null!;

                if (item.DiscountId.HasValue && discountsMap.TryGetValue(item.DiscountId.Value, out discount))
                {
                    IEnumerable<DiscountTierResponse>? tiers = null;
                    if (discount.DiscountType == DiscountType.Tiered)
                    {
                        groupedTiers.TryGetValue(discount.Id, out tiers);
                    }
                    
                    if (discount.DiscountType is DiscountType.Percentage or
                                                 DiscountType.FixedAmount or
                                                 DiscountType.BuyOneGetOne or
                                                 DiscountType.Tiered)
                    {
                        discountItemDTO = new DiscountItemDTO(
                            discount.Id,
                            discount.Name,
                            discount.DiscountType,
                            discount.DiscountScope,
                            discount.DiscountValueType,
                            discount.Value,
                            discount.ValidFrom,
                            discount.ValidTo,
                            discount.IsActive,
                            tiers is null ? null : tiers.ToList()
                        );
                        items.Add(new OrderItemDTO(
                            item.Id,
                            item.ProductId,
                            productName,
                            item.Quantity,
                            item.Price,
                            discountItemDTO
                        ));
                    }
                    else if (discount.DiscountType == DiscountType.Bundle)
                    {
                        bool bundleWithItem = bundledItems
                            .Where(bi => bi.Id == discount.Id)
                            .Any(bi => bi.BundleItems.Any(bid => bid.ProductId == item.ProductId));

                        if (bundleWithItem || !bundledItems.Any(bi => bi.Id == discount.Id))
                        {
                            bundledItems.Add(new BundleItemsDTO(
                                discount.Id,
                                discount.Name,
                                discount.DiscountType,
                                discount.DiscountScope,
                                discount.DiscountValueType,
                                discount.Value,
                                discount.ValidFrom,
                                discount.ValidTo,
                                discount.IsActive,
                                new List<BundleItemDTO> {
                                    new BundleItemDTO(
                                        item.Id,
                                        item.ProductId,
                                        productName,
                                        item.Quantity,
                                        item.Price
                                    )}
                            ));
                        }
                        else
                        {
                            var bundle = bundledItems.First(bi => bi.Id == discount.Id && !bi.BundleItems.Any(bid => bid.ProductId == item.ProductId));
                            bundle.BundleItems.Add(new BundleItemDTO(
                                item.Id,
                                item.ProductId,
                                productName,
                                item.Quantity,
                                item.Price
                            ));
                        }
                    }
                }
                else
                {
                    items.Add(new OrderItemDTO(
                        item.Id,
                        item.ProductId,
                        productName,
                        item.Quantity,
                        item.Price,
                        null
                    ));
                }
            }

            var resultDTO = new OrderCompleteDTO(
                order.Id,
                user.Id,
                user.Name,
                order.OrderType,
                new OrderAddressResponse(
                    order.Address.Number,
                    order.Address.Street,
                    order.Address.Neighbourhood,
                    order.Address.City,
                    order.Address.Country,
                    order.Address.Complement,
                    order.Address.CEP
                ),
                order.PaymentMethod,
                order.TotalPrice,
                order.OrderDate,
                order.Confirmation,
                order.Status,
                orderAppliedDiscount,
                items,
                bundledItems
            );

            failed = "";
            return Result<OrderCompleteDTO>.Success(resultDTO);
        }

        private Result<Order> GetOrderFromcCache(int orderId)
        {
            return _cacheHandler.GetFromCache<Order, Address, CardInformation, Order>(orderId, nameof(Address), nameof(CardInformation),
                (cache, prop) => new Address().Create(
                    Convert.ToInt32(cache[$"{prop}_Number"]),
                    Convert.ToString(cache[$"{prop}_Street"])!,
                    Convert.ToString(cache[$"{prop}_Neighbourhood"])!,
                    Convert.ToString(cache[$"{prop}_City"])!,
                    Convert.ToString(cache[$"{prop}_Country"])!,
                    cache.GetNullableString($"{prop}_Complement"),
                    Convert.ToString(cache[$"{prop}_CEP"])!
                ).GetValue(),
                (cache, prop) => cache.ContainsKey($"{prop}_CardNumber")
                    ? new CardInformation().Create(
                        Convert.ToString(cache[$"{prop}_CardHolderName"])!,
                        Convert.ToString(cache[$"{prop}_CardNumber"])!,
                        Convert.ToString(cache[$"{prop}_ExpirationMonth"])!,
                        Convert.ToString(cache[$"{prop}_ExpirationYear"])!,
                        (CardFlag)Convert.ToInt32(cache[$"{prop}_CardFlag"]),
                        Convert.ToString(cache[$"{prop}_Last4Digits"])!
                    ).GetValue()
                    : null,
                (cache, address, cardInfo) => new Order().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToInt32(cache["UserId"]),
                    (OrderType)Convert.ToInt32(cache["OrderType"]),
                    address!,
                    cache.GetNullablePaymentMethod("PaymentMethod"),
                    cache.GetNullableDecimal("TotalPrice"),
                    cache.GetNullableDateTime("OrderDate"),
                    Convert.ToBoolean(cache["Confirmation"]),
                    Convert.ToString(cache["Status"])!,
                    cache.GetNullableInt("DiscountId"),
                    cardInfo
                ).GetValue()
            );
        }

        private Result<User> GetUserFromCache(int userId)
        {
            return _cacheHandler.GetFromCache<User, User>(userId, cache =>
                new User().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToString(cache["Name"])!,
                    Convert.ToString(cache["Email"])!,
                    Convert.ToString(cache["PhoneNumber"])!,
                    Convert.ToString(cache["Password"])!
                ).GetValue());
        }

        private Result<List<OrderItem>> GetOrderItemsFromCache(int orderId)
        {
            return _cacheHandler.ListFromCacheByProperty<OrderItem, OrderItem>(nameof(OrderItem.OrderId), orderId,
                cache => new OrderItem().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToDecimal(cache["Price"]),
                    Convert.ToInt32(cache["Quantity"]),
                    Convert.ToInt32(cache["ProductId"]),
                    Convert.ToInt32(cache["OrderId"]),
                    cache.GetNullableInt("DiscountId")
                ).GetValue());
        }

        private Result<List<Product>> GetProductsFromCache(List<int> productIds)
        {
            return _cacheHandler.ListFromCacheByPropertyIn<Product, Product>(nameof(Product.Id), productIds.Cast<object>(),
                cache => new Product().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToString(cache["Name"])!,
                    Convert.ToDecimal(cache["Price"]),
                    Convert.ToString(cache["Description"])!,
                    Convert.ToInt32(cache["Stock"])
                ).GetValue());
        }

        private Result<List<Discount>> GetOrderItemDiscountsFromCache(List<int?> discountIds)
        {
            return _cacheHandler.ListFromCacheByPropertyIn<Discount, Discount>(nameof(Discount.Id), discountIds.Cast<object>(),
                cache => new Discount().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToString(cache["Name"])!,
                    (DiscountType)Convert.ToInt32(cache["DiscountType"]),
                    (DiscountScope)Convert.ToInt32(cache["DiscountScope"]),
                    cache.GetNullableDiscountValueType("DiscountValueType"),
                    cache.GetNullableDecimal("Value"),
                    cache.GetNullableDateTime("ValidFrom"),
                    cache.GetNullableDateTime("ValidTo"),
                    Convert.ToBoolean(cache["IsActive"])
                ).GetValue());
        }

        private Result<List<DiscountTier>> GetTiersFromCache(int discountId)
        {
            return _cacheHandler.ListFromCacheByProperty<DiscountTier, DiscountTier>(nameof(DiscountTier.DiscountId), discountId,
                cache => new DiscountTier().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToString(cache["Name"])!,
                    Convert.ToInt32(cache["MinQuantity"]),
                    Convert.ToDecimal(cache["Value"]),
                    Convert.ToInt32(cache["DiscountId"])
                ).GetValue());
        }

        private Result<Discount> GetOrderDiscountFromCache(int discountId)
        {
            return _cacheHandler.GetFromCacheByProperty<Discount, Discount>(nameof(Discount.Id), discountId,
                cache => new Discount().Create(
                    Convert.ToInt32(cache["Id"]),
                    Convert.ToString(cache["Name"])!,
                    (DiscountType)Convert.ToInt32(cache["DiscountType"]),
                    (DiscountScope)Convert.ToInt32(cache["DiscountScope"]),
                    cache.GetNullableDiscountValueType("DiscountValueType"),
                    cache.GetNullableDecimal("Value"),
                    cache.GetNullableDateTime("ValidFrom"),
                    cache.GetNullableDateTime("ValidTo"),
                    Convert.ToBoolean(cache["IsActive"])
                ).GetValue());
        }
    }
}
