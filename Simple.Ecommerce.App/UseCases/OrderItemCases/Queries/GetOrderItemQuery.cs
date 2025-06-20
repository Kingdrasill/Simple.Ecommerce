using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Queries
{
    public class GetOrderItemQuery : IGetOrderItemQuery
    {
        private readonly IOrderItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetOrderItemQuery(
            IOrderItemRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderItemResponse>> Execute(int id, bool NoTracking = true)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<OrderItem, OrderItemResponse>(id, cache =>
                    new OrderItemResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToDecimal(cache["Price"]),
                        Convert.ToInt32(cache["Quantity"]),
                        Convert.ToInt32(cache["OrderId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository(id, NoTracking);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<OrderItem>();

            return repoResponse;
        }

        private async Task<Result<OrderItemResponse>> GetFromRepository(int id, bool NoTracking)
        {
            var getResult = await _repository.Get(id, NoTracking);

            if (getResult.IsFailure)
            {
                return Result<OrderItemResponse>.Failure(getResult.Errors!);
            }

            var cartItem = getResult.GetValue();

            var response = new OrderItemResponse(
                cartItem.Id,
                cartItem.ProductId,
                cartItem.Price,
                cartItem.Quantity,
                cartItem.OrderId
            );

            return Result<OrderItemResponse>.Success(response);
        }
    }
}
