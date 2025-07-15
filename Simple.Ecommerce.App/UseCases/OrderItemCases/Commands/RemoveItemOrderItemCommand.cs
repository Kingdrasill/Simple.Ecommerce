using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveItemOrderItemCommand : IRemoveItemOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveItemOrderItemCommand(
            IOrderItemRepository repository, 
            IProductRepository productRepository, 
            IOrderRepository orderRepository, 
            IDiscountRepository discountRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _discountRepository = discountRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderItemResponse?>> Execute(OrderItemRequest request)
        {
            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<OrderItemResponse?>.Failure(getProduct.Errors!);
            }

            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderItemResponse?>.Failure(getOrder.Errors!);
            }

            var getDiscount = request.DiscountId is null ? null : await _discountRepository.Get(request.DiscountId.Value);
            if (getDiscount is not null && getDiscount.IsFailure)
            {
                return Result<OrderItemResponse?>.Failure(getDiscount.Errors!);
            }

            OrderItemResponse? response = null;

            var getOrderItem = await _repository.GetByOrderIdAndProductId(request.OrderId, request.ProductId);
            if (getOrderItem.IsSuccess)
            {
                var orderItem = getOrderItem.GetValue();
                orderItem.Update(-request.Quantity, getProduct.GetValue().Price, request.DiscountId, request.Override);

                if (orderItem.Quantity <= 0)
                {
                    var deleteResult = await _repository.Delete(orderItem.Id);
                    if (deleteResult.IsFailure)
                    {
                        return Result<OrderItemResponse?>.Failure(deleteResult.Errors!);
                    }
                }
                else
                {
                    var validateOrderItem = orderItem.Validate();
                    if (validateOrderItem.IsFailure)
                    {
                        return Result<OrderItemResponse?>.Failure(validateOrderItem.Errors!);
                    }

                    var updateResult = await _repository.Update(orderItem);
                    if (updateResult.IsFailure)
                    {
                        return Result<OrderItemResponse?>.Failure(updateResult.Errors!);
                    }

                    var orderItemUpdated = updateResult.GetValue();

                    response = new OrderItemResponse(
                        orderItemUpdated.Id,
                        orderItemUpdated.Price,
                        orderItemUpdated.Quantity,
                        orderItemUpdated.ProductId,
                        orderItemUpdated.OrderId,
                        orderItemUpdated.DiscountId
                    );
                }
            }
            else
            {
                return Result<OrderItemResponse?>.Failure(new List<Error> { new("RemoveItemOrderItemCommand.NotFound", "Item do pedido não encontrado!") });
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<OrderItemResponse?>.Success(response);
        }
    }
}
