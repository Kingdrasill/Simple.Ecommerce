using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveItemOrderItemCommand : IRemoveItemOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveItemOrderItemCommand(
            IOrderItemRepository repository, 
            IProductRepository productRepository, 
            IOrderRepository orderRepository, 
            IProductDiscountRepository productDiscountRepository,
            IDiscountRepository discountRepository,
            IDiscountBundleItemRepository discountBundleItemRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _productDiscountRepository = productDiscountRepository;
            _discountRepository = discountRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderItemResponse?>> Execute(OrderItemRequest request)
        {
            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderItemResponse?>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not OrderLock.Unlock)
            {
                return Result<OrderItemResponse?>.Failure(new List<Error> { new("RemoveItemOrderItemCommand.OrderLocked", "Não é possível mudar os dados do pedido!") });
            }

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<OrderItemResponse?>.Failure(getProduct.Errors!);
            }
            var product = getProduct.GetValue();

            Discount? discount = null;
            var getProductDiscount = request.ProductDiscountId is null ? null : await _productDiscountRepository.Get(request.ProductDiscountId.Value);
            if (getProductDiscount is not null)
            {
                if (getProductDiscount.IsFailure)
                {
                    return Result<OrderItemResponse?>.Failure(getProductDiscount.Errors!);
                }
                var productDiscount = getProductDiscount.GetValue();
                if (productDiscount.ProductId != request.ProductId)
                {
                    return Result<OrderItemResponse?>.Failure(new List<Error> { new("ChangeDiscountOrderItemCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} é para o produto {productDiscount.ProductId} não para o produto {request.ProductId}!") });
                }

                var getDiscount = await _discountRepository.Get(productDiscount.DiscountId);
                if (getDiscount.IsFailure)
                {
                    return Result<OrderItemResponse?>.Failure(getDiscount.Errors!);
                }
                discount = getDiscount.GetValue();

                var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(getOrder.GetValue().Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    return Result<OrderItemResponse?>.Failure(getOrderItemsDiscountInfo.Errors!);
                }

                var productValidation = await ProductDiscountValidation.Validate(_discountBundleItemRepository, discount, product, getOrderItemsDiscountInfo.GetValue(), "AddItemOrderItemCommand", product.Id);
                if (productValidation.IsFailure)
                {
                    return Result<OrderItemResponse?>.Failure(productValidation.Errors!);
                }
            }

            OrderItemResponse? response = null;
            var getOrderItem = await _repository.GetByOrderIdAndProductId(request.OrderId, request.ProductId);
            if (getOrderItem.IsSuccess)
            {
                var orderItem = getOrderItem.GetValue();
                orderItem.Update(
                    -request.Quantity, 
                    product.Price, 
                    discount is null ? null : discount.Id, 
                    request.Override
                );
                if (orderItem.Validate() is { IsFailure: true } result)
                {
                    return Result<OrderItemResponse?>.Failure(result.Errors!);
                }

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
