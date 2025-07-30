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
    public class AddItemOrderItemCommand : IAddItemOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;


        public AddItemOrderItemCommand(
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

        public async Task<Result<OrderItemResponse>> Execute(OrderItemRequest request)
        {
            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderItemResponse>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not OrderLock.Unlock)
            {
                return Result<OrderItemResponse>.Failure(new List<Error> { new("AddItemOrderItemCommand.OrderLocked", "Não é possível mudar os dados do pedido!") });
            }

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<OrderItemResponse>.Failure(getProduct.Errors!);
            }
            var product = getProduct.GetValue();

            Discount? discount = null;
            var getProductDiscount = request.ProductDiscountId is null ? null : await _productDiscountRepository.Get(request.ProductDiscountId.Value);
            if (getProductDiscount is not null)
            {
                if (getProductDiscount.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(getProductDiscount.Errors!);
                }
                var productDiscount = getProductDiscount.GetValue();
                if (productDiscount.ProductId != product.Id)
                {
                    return Result<OrderItemResponse>.Failure(new List<Error> { new("AddItemOrderItemCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} é para o produto {productDiscount.ProductId} não para o produto {product.Id}!") });
                }

                var getDiscount = await _discountRepository.Get(productDiscount.DiscountId);
                if (getDiscount.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(getDiscount.Errors!);
                }
                discount = getDiscount.GetValue();

                var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(order.Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(getOrderItemsDiscountInfo.Errors!);
                }

                var productValidation = await ProductDiscountValidation.Validate(_discountBundleItemRepository, discount, product, getOrderItemsDiscountInfo.GetValue(), "AddItemOrderItemCommand", product.Id);
                if (productValidation.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(productValidation.Errors!);
                }
            }

            OrderItemResponse response = new OrderItemResponse(0, 0, 0, 0, 0, null);
            var getOrderItem = await _repository.GetByOrderIdAndProductId(order.Id, product.Id);
            if (getOrderItem.IsSuccess)
            {
                var orderItem = getOrderItem.GetValue();
                orderItem.Update(
                    request.Quantity, 
                    getProduct.GetValue().Price, 
                    discount is null ? null : discount.Id, 
                    request.Override
                );
                if (orderItem.Validate() is { IsFailure: true } result)
                {
                    return Result<OrderItemResponse>.Failure(result.Errors!);
                }

                var updateResult = await _repository.Update(orderItem);
                if (updateResult.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(updateResult.Errors!);
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
            else
            {
                var instance = new OrderItem().Create(
                    0,
                    getProduct.GetValue().Price,
                    request.Quantity,
                    request.ProductId,
                    request.OrderId,
                    discount is null ? null : discount.Id
                );
                if (instance.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(instance.Errors!);
                }

                var createResult = await _repository.Create(instance.GetValue());
                if (createResult.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(createResult.Errors!);
                }
                var orderItem = createResult.GetValue();

                response = new OrderItemResponse(
                    orderItem.Id,
                    orderItem.Price,
                    orderItem.Quantity,
                    orderItem.ProductId,
                    orderItem.OrderId,
                    orderItem.DiscountId
                );
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<OrderItemResponse>.Success(response);
        }
    }
}
