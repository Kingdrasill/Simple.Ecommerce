using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Queries
{
    public class GetOrderItemQuery : IGetOrderItemQuery
    {
        private readonly IOrderItemRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetOrderItemQuery(
            IOrderItemRepository repository,
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

        public async Task<Result<OrderItemResponse>> Execute(int id)
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

            var repoResponse = await _repositoryHandler.GetFromRepository<OrderItem, OrderItemResponse>(
                id,
                async (id) => await _repository.Get(id),
                orderItem => new OrderItemResponse(
                    orderItem.Id,
                    orderItem.ProductId,
                    orderItem.Price,
                    orderItem.Quantity,
                    orderItem.OrderId
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<OrderItem>();

            return repoResponse;
        }
    }
}
