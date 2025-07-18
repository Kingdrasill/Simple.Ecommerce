using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class GetPaymentMethodOrderQuery : IGetPaymentInformationOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPaymentMethodOrderQuery(
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

        public async Task<Result<OrderPaymentInformationResponse>> Execute(int orderId)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Order, PaymentInformationOrderResponse, OrderPaymentInformationResponse>(orderId, nameof(PaymentInformation),
                    (cache, propName) => {
                        if (cache.TryGetValue($"{propName}_PaymentMethod", out var cardHolderName) && cardHolderName is not null)
                            return new PaymentInformationOrderResponse(
                                (PaymentMethod)Convert.ToInt32(cache[$"{propName}_PaymentMethod"]),
                                cache.GetNullableString($"{propName}_PaymentName"),
                                (PaymentMethod)Convert.ToInt32(cache[$"{propName}_PaymentMethod"]) is not (PaymentMethod.CreditCard or PaymentMethod.DebitCard)
                                    ? cache.GetNullableString($"{propName}_PaymentKey")
                                    : null,
                                cache.GetNullableString($"{propName}_ExpirationMonth"),
                                cache.GetNullableString($"{propName}_ExpirationYear"),
                                cache.GetNullableCardFlag($"{propName}_CardFlag"),
                                cache.GetNullableString($"{propName}_Last4Digits")
                            );
                        return null;
                    }, 
                    (cache, cardInformation) => new OrderPaymentInformationResponse(
                        Convert.ToInt32(cache["Id"]),
                        cardInformation
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Order, OrderPaymentInformationResponse>(
                orderId,
                async (id) => await _repository.Get(id),
                order => {
                    if (order.PaymentInformation is null)
                        return new OrderPaymentInformationResponse(order.Id, null);
                    return new OrderPaymentInformationResponse(
                        orderId,
                        order.PaymentInformation is null 
                            ? null 
                            : new PaymentInformationOrderResponse(
                                order.PaymentInformation.PaymentMethod,
                                order.PaymentInformation.PaymentName,
                                order.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or PaymentMethod.DebitCard)
                                    ? order.PaymentInformation.PaymentKey
                                    : null,
                                order.PaymentInformation.ExpirationMonth,
                                order.PaymentInformation.ExpirationYear,
                                order.PaymentInformation.CardFlag,
                                order.PaymentInformation.Last4Digits
                            )
                    );
                });
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Order>();

            return repoResponse;
        }
    }
}
