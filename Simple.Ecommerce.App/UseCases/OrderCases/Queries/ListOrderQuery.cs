using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class ListOrderQuery : IListOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListOrderQuery(
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

        public async Task<Result<List<OrderResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Order, OrderAddressResponse, PaymentInformationOrderResponse, OrderResponse>(nameof(Address), nameof(PaymentInformation),
                    (cache, prop) => new OrderAddressResponse(
                        Convert.ToInt32(cache[$"{prop}_Number"]),
                        Convert.ToString(cache[$"{prop}_Street"])!,
                        Convert.ToString(cache[$"{prop}_Neighbourhood"])!,
                        Convert.ToString(cache[$"{prop}_City"])!,
                        Convert.ToString(cache[$"{prop}_Country"])!,
                        cache.GetNullableString($"{prop}_Complement"),
                        Convert.ToString(cache[$"{prop}_CEP"])!
                    ),
                    (cache, prop) => cache.ContainsKey($"{prop}_PaymentMethod")
                        ? new PaymentInformationOrderResponse(
                            (PaymentMethod)Convert.ToInt32(cache[$"{prop}_PaymentMethod"]),
                            cache.GetNullableString($"{prop}_PaymentName"),
                            (PaymentMethod)Convert.ToInt32(cache[$"{prop}_PaymentMethod"]) is not (PaymentMethod.CreditCard or PaymentMethod.DebitCard)
                                ? cache.GetNullableString($"{prop}_PaymentKey")
                                : null,
                            cache.GetNullableString($"{prop}_ExpirationMonth"),
                            cache.GetNullableString($"{prop}_ExpirationYear"),
                            cache.GetNullableCardFlag($"{prop}_CardFlag"),
                            cache.GetNullableString($"{prop}_Last4Digits")
                        )
                        : null,
                    (cache, address, paymentInfo) => new OrderResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["UserId"]),
                        (OrderType)Convert.ToInt32(cache["OrderType"])!,
                        address!,
                        paymentInfo,
                        cache.GetNullableDecimal("TotalPrice"),
                        cache.GetNullableDateTime("OrderDate"),
                        Convert.ToBoolean(cache["Confirmation"]),
                        Convert.ToString(cache["Status"])!,
                        cache.GetNullableInt("DiscountId")
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.ListFromRepository<Order, OrderResponse>(
                async () => await _repository.List(),
                order => {
                    var addressResponse = new OrderAddressResponse(
                        order.Address.Number,
                        order.Address.Street,
                        order.Address.Neighbourhood,
                        order.Address.City,
                        order.Address.Country,
                        order.Address.Complement,
                        order.Address.CEP
                    );
                    return new OrderResponse(
                        order.Id,
                        order.UserId,
                        order.OrderType,
                        addressResponse,
                        order.PaymentInformation is null
                            ? null
                            : new PaymentInformationOrderResponse(
                                order.PaymentInformation.PaymentMethod,
                                order.PaymentInformation.PaymentName,
                                order.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or PaymentMethod.CreditCard)
                                    ? order.PaymentInformation.PaymentKey
                                    : null,
                                order.PaymentInformation.ExpirationMonth,
                                order.PaymentInformation.ExpirationYear,
                                order.PaymentInformation.CardFlag,
                                order.PaymentInformation.Last4Digits
                            ),
                        order.TotalPrice,
                        order.OrderDate,
                        order.Confirmation,
                        order.Status,
                        order.DiscountId
                    );
                });
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Order>();

            return repoResponse;
        }
    }
}
