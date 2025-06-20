using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class AddItemsOrderItemCommand : IAddItemsOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddItemsOrderItemCommand(
            IOrderItemRepository repository, 
            IOrderRepository orderRepository, 
            IProductRepository productRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderItemsResponse>> Execute(OrderItemsRequest request)
        {
            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderItemsResponse>.Failure(getOrder.Errors!);
            }

            OrderItemsResponse response = new OrderItemsResponse(new List<OrderItemResponse>());
            foreach (var orderItemRequest in request.OrderItems)
            {
                if (request.OrderId != getOrder.GetValue().Id)
                {
                    return Result<OrderItemsResponse>.Failure(new List<Error> { new("AddItemsOrderItemCommand.NotRelated.OrderId", "Um dos itens passado é de um pedido diferente!") });
                }

                var getProduct = await _productRepository.Get(orderItemRequest.ProductId);
                if (getProduct.IsFailure)
                {
                    return Result<OrderItemsResponse>.Failure(getProduct.Errors!);
                }

                var instance = new OrderItem().Create(
                    0,
                    orderItemRequest.ProductId,
                    orderItemRequest.Price,
                    orderItemRequest.Quantity,
                    orderItemRequest.OrderId
                );
                if (instance.IsFailure)
                {
                    return Result<OrderItemsResponse>.Failure(instance.Errors!);
                }

                var createResult = await _repository.Create(instance.GetValue());
                if (createResult.IsFailure)
                {
                    return Result<OrderItemsResponse>.Failure(createResult.Errors!);
                }

                var orderItem = createResult.GetValue();

                response.OrderItems.Add(new OrderItemResponse(
                    orderItem.Id,
                    orderItem.ProductId,
                    orderItem.Price,
                    orderItem.Quantity,
                    orderItem.OrderId
                ));
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<OrderItemsResponse>.Success(response);
        }
    }
}
