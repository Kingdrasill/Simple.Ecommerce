using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class ChangeDiscountOrderItemCommand : IChangeDiscountOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangeDiscountOrderItemCommand(
            IOrderItemRepository repository, 
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IProductDiscountRepository productDiscountRepository,
            IDiscountRepository discountRepository, 
            IDiscountBundleItemRepository discountBundleItemRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productDiscountRepository = productDiscountRepository;
            _discountRepository = discountRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(OrderItemDiscountRequest request)
        {
            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<bool>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not OrderLock.Unlock)
            {
                return Result<bool>.Failure(new List<Error> { new("ChangeDiscountOrderItemCommand.OrderLocked", "Não é possível mudar os dados do pedido!") });
            }

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<bool>.Failure(getProduct.Errors!);
            }
            var product = getProduct.GetValue();

            var getOrderItem = await _repository.GetByOrderIdAndProductId(request.OrderId, request.ProductId);
            if (getOrderItem.IsFailure)
            {
                return Result<bool>.Failure(getOrderItem.Errors!);
            }
            var orderItem = getOrderItem.GetValue();

            Discount? discount = null;
            var getProductDiscount = request.ProductDiscountId is null ? null : await _productDiscountRepository.Get(request.ProductDiscountId.Value);
            if (getProductDiscount is not null)
            {
                if (getProductDiscount.IsFailure)
                {
                    return Result<bool>.Failure(getProductDiscount.Errors!);
                }
                var productDiscount = getProductDiscount.GetValue();
                if (productDiscount.ProductId != orderItem.ProductId)
                {
                    return Result<bool>.Failure(new List<Error> { new("ChangeDiscountOrderItemCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} é para o produto {productDiscount.ProductId} não para o produto {orderItem.ProductId}!") });
                }

                var getDiscount = await _discountRepository.Get(productDiscount.DiscountId);
                if (getDiscount.IsFailure)
                {
                    return Result<bool>.Failure(getDiscount.Errors!);
                }
                discount = getDiscount.GetValue();

                var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(getOrder.GetValue().Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    return Result<bool>.Failure(getOrderItemsDiscountInfo.Errors!);
                }

                var productValidation = await ProductDiscountValidation.Validate(_discountBundleItemRepository, discount, product, getOrderItemsDiscountInfo.GetValue(), "AddItemOrderItemCommand", product.Id);
                if (productValidation.IsFailure)
                {
                    return Result<bool>.Failure(productValidation.Errors!);
                }
            }

            orderItem.UpdateDiscountId(discount is null ? null : discount.Id);
            if (orderItem.Validate() is { IsFailure: true } result)
            {
                return Result<bool>.Failure(result.Errors!);
            }

            var updateResult = await _repository.Update(orderItem);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<bool>.Success(true);
        }
    }
}
