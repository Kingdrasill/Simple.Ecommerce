using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Queries
{
    public class ListOrderQuery : IListOrderQuery
    {
        private readonly IOrderRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListOrderQuery(
            IOrderRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<OrderResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Order, AddressResponse, OrderResponse>(nameof(Address),
                    (cache, propName) => new AddressResponse(
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
                        Convert.ToDateTime(cache["OrderDate"]),
                        Convert.ToInt32(cache["UserId"]),
                        Convert.ToDecimal(cache["TotalPrice"]),
                        (OrderType)Convert.ToInt32(cache["OrderType"])!,
                        Convert.ToBoolean(cache["Confirmation"]),
                        Convert.ToString(cache["Status"])!,
                        cache.GetNullableString("PaymentMethod"),
                        address
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository();
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
                var addressResponse = new AddressResponse(
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
                    order.OrderDate,
                    order.UserId,
                    order.TotalPrice,
                    order.OrderType,
                    order.Confirmation,
                    order.Status,
                    order.PaymentMethod,
                    addressResponse
                ));
            }

            return Result<List<OrderResponse>>.Success(response);
        }
    }
}
