using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

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
                var cacheResponse = _cacheHandler.ListFromCache<Order, OrderAddressResponse, OrderResponse>(nameof(Address),
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
                        Convert.ToString(cache["Status"])!
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
                        order.PaymentMethod,
                        order.TotalPrice,
                        order.OrderDate,
                        order.Confirmation,
                        order.Status
                    );
                });
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Order>();

            return repoResponse;
        }

        private async Task<Result<List<OrderResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<OrderResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<OrderResponse>();
            foreach (var order in listResult.GetValue())
            {
                var addressResponse = new OrderAddressResponse(
                    order.Address.Number,
                    order.Address.Street,
                    order.Address.Neighbourhood,
                    order.Address.City,
                    order.Address.Country,
                    order.Address.Complement,
                    order.Address.CEP
                );

                response.Add(new OrderResponse(
                    order.Id,
                    order.UserId,
                    order.OrderType,
                    addressResponse,
                    order.PaymentMethod,
                    order.TotalPrice,
                    order.OrderDate,
                    order.Confirmation,
                    order.Status
                ));
            }

            return Result<List<OrderResponse>>.Success(response);
        }
    }
}
