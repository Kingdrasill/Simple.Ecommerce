using Cache.Library.Core;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class GetOrderQuery : IGetOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetOrderQuery(
            IOrderRepository repository,
            IRepositoryHandler repositoryHandler,
            ICache cache,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Order, OrderAddressResponse, OrderResponse>(id, nameof(Address),
                    (cache, propName) => new OrderAddressResponse(
                        Convert.ToInt32(cache[$"{propName}_Number"]),
                        Convert.ToString(cache[$"{propName}_Street"])!,
                        Convert.ToString(cache[$"{propName}_Neighbourhood"])!,
                        Convert.ToString(cache[$"{propName}_City"])!,
                        Convert.ToString(cache[$"{propName}_Country"])!,
                        cache.GetNullableString($"{propName}_Complement"),
                        Convert.ToString(cache[$"{propName}_CEP"])!
                    ),
                    (cache, address) => new OrderResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["UserId"]),
                        (OrderType)Convert.ToInt32(cache["OrderType"])!,
                        address,
                        cache.GetNullablePaymentMethod("PaymentMethod"),
                        cache.GetNullableDecimal("TotalPrice"),
                        cache.GetNullableDateTime("OrderDate"),
                        Convert.ToBoolean(cache["Confirmation"]),
                        Convert.ToString(cache["Status"])!,
                        cache.GetNullableInt("DiscountId")
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Order, OrderResponse>(
                id,
                async (id) => await _repository.Get(id),
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
                        order.PaymentMethod,
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
