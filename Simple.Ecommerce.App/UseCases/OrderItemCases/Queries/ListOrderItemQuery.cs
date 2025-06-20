using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Queries
{
    public class ListOrderItemQuery : IListOrderItemQuery
    {
        private readonly IOrderItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListOrderItemQuery(
            IOrderItemRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<OrderItemResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<OrderItem, OrderItemResponse>(cache =>
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

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<OrderItem>();

            return repoResponse;
        }

        private async Task<Result<List<OrderItemResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<OrderItemResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<OrderItemResponse>();
            foreach (var cartItem in listResult.GetValue())
            {
                response.Add(new OrderItemResponse(
                    cartItem.Id,
                    cartItem.ProductId,
                    cartItem.Price,
                    cartItem.Quantity,
                    cartItem.OrderId
                ));
            }

            return Result<List<OrderItemResponse>>.Success(response);
        }
    }
}
