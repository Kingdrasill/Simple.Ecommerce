using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.CardInformationContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class GetPaymentMethodOrderQuery : IGetPaymentMethodOrderQuery
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

        public async Task<Result<OrderPaymentMethodResponse>> Execute(int orderId)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Order, OrderCardInformationResponse, OrderPaymentMethodResponse>(orderId, nameof(CardInformation),
                    (cache, propName) => {
                        if (cache.TryGetValue($"{propName}_CardHolderName", out var cardHolderName) && cardHolderName is not null)
                                return new OrderCardInformationResponse(
                                    Convert.ToString(cardHolderName)!,
                                    Convert.ToString(cache[$"{propName}_ExpirationMonth"])!,
                                    Convert.ToString(cache[$"{propName}_ExpirationYear"])!,
                                    (CardFlag)Convert.ToInt32(cache[$"{propName}_CardFlag"]),
                                    Convert.ToString(cache[$"{propName}_Last4Digits"])!
                                );
                        return null;
                    }, 
                    (cache, cardInformation) => new OrderPaymentMethodResponse(
                        Convert.ToInt32(cache["Id"]),
                        cache.GetNullablePaymentMethod("PaymentMethod"),
                        cardInformation
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Order, OrderPaymentMethodResponse>(
                orderId,
                async (id) => await _repository.Get(id),
                order => {
                    if (order.PaymentMethod is null)
                        return new OrderPaymentMethodResponse(order.Id, null, null);
                    return new OrderPaymentMethodResponse(
                        orderId,
                        order.PaymentMethod,
                        order.CardInformation is null ? null : new OrderCardInformationResponse(
                            order.CardInformation.CardHolderName,
                            order.CardInformation.ExpirationMonth,
                            order.CardInformation.ExpirationYear,
                            order.CardInformation.CardFlag,
                            order.CardInformation.Last4Digits
                    ));
                });
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Order>();

            return repoResponse;
        }
    }
}
