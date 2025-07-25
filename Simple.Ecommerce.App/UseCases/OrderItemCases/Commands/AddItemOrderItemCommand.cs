using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class AddItemOrderItemCommand : IAddItemOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;


        public AddItemOrderItemCommand(
            IOrderItemRepository repository, 
            IProductRepository productRepository, 
            IOrderRepository orderRepository, 
            IDiscountRepository discountRepository,
            IDiscountBundleItemRepository discountBundleItemRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
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

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<OrderItemResponse>.Failure(getProduct.Errors!);
            }
            var product = getProduct.GetValue();

            var getDiscount = request.DiscountId is null ? null : await _discountRepository.Get(request.DiscountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                    return Result<OrderItemResponse>.Failure(getDiscount.Errors!);

                var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(order.Id);

                var validateResult = await ValidateProductDiscount(getDiscount.GetValue(), product, getOrderItemsDiscountInfo.GetValue());
                if (validateResult.IsFailure)
                {
                    return Result<OrderItemResponse>.Failure(validateResult.Errors!);
                }
            }

            OrderItemResponse response = new OrderItemResponse(0, 0, 0, 0, 0, null);

            var getOrderItem = await _repository.GetByOrderIdAndProductId(order.Id, product.Id);
            if (getOrderItem.IsSuccess)
            {
                var orderItem = getOrderItem.GetValue();
                orderItem.Update(request.Quantity, getProduct.GetValue().Price, request.DiscountId, request.Override);
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
                    request.DiscountId
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

        private async Task<Result<bool>> ValidateProductDiscount(Discount discount, Product product, List<OrderItemDiscountInfoDTO> orderItemsDiscountInfo)
        {
            List<Error> errors = new();

            if (discount.DiscountScope != DiscountScope.Product)
                errors.Add(new("AddItemOrderItemCommand.InvalidDiscountScope", "O desconto não é aplicável a produtos!"));

            if (discount.DiscountType == DiscountType.FirstPurchase)
                errors.Add(new("AddItemOrderItemCommand.InvalidDiscountType", "O desconto não é aplicável a produtos!"));

            if (discount.ValidFrom > DateTime.UtcNow)
                errors.Add(new("AddItemOrderItemCommand.DiscountNotValidYet", "O desconto ainda não está válido!"));

            if (discount.ValidTo < DateTime.UtcNow)
                errors.Add(new("AddItemOrderItemCommand.DiscountExpired", "O desconto já expirou!"));

            if (!discount.IsActive)
                errors.Add(new("AddItemOrderItemCommand.InactiveDiscount", "O desconto não está ativo!"));

            if (discount.DiscountType == DiscountType.Bundle)
            {
                var getProductIdsOfBundle = await _discountBundleItemRepository.GetProductIdsByDiscountId(discount.Id);
                if (getProductIdsOfBundle.IsFailure)
                {
                    return Result<bool>.Failure(getProductIdsOfBundle.Errors!);
                }

                var productIdsOfBundle = getProductIdsOfBundle.GetValue();

                var productsBundleInserted = orderItemsDiscountInfo.Where(tmp => productIdsOfBundle.Contains(tmp.ProductId));

                foreach (var bundleItem in productsBundleInserted)
                {
                    if (bundleItem.DiscountId is null)
                        continue;

                    if (bundleItem.DiscountType != DiscountType.Bundle)
                        errors.Add(new("AddItemOrderItemCommand.ConflictDiscountType", $"Um produto do desconto de pacote {discount.Id} já está utilizando um desconto que não é de pacote!"));
                    else if (bundleItem.DiscountId != discount.Id)
                        errors.Add(new("AddItemOrderItemCommand.ConflictDiscountBundle", $"Um produto do desconto de pacote {discount.Id} já está utilizando um desconto para outro desconto de pacote!"));
                }
            }
            else
            {
                var getProductBundles = await _discountBundleItemRepository.GetByProductId(product.Id);
                if (getProductBundles.IsFailure)
                {
                    return Result<bool>.Failure(getProductBundles.Errors!);
                }

                foreach (var bundle in getProductBundles.GetValue())
                {
                    var getProductsIdsOfBundle = await _discountBundleItemRepository.GetProductIdsByDiscountId(bundle.DiscountId);
                    if (getProductsIdsOfBundle.IsFailure)
                    {
                        return Result<bool>.Failure(getProductsIdsOfBundle.Errors!);
                    }

                    var productsIdsOfBundle = getProductsIdsOfBundle.GetValue();
                    var productsBundleInserted = orderItemsDiscountInfo.Where(tmp => productsIdsOfBundle.Contains(tmp.ProductId));

                    foreach (var bundleItem in productsBundleInserted)
                    {
                        if (bundleItem.DiscountId is null)
                            continue;

                        if (bundleItem.DiscountType == DiscountType.Bundle)
                            errors.Add(new("AddItemOrderItemCommand.ConflictDiscountType", "Um dos produtos do pedido está utilizando o desconto de pacote que este produto faz parte mas está usando outro desconto!"));
                    }
                }
            }

            if (errors.Count != 0)
            {
                return Result<bool>.Failure(errors);
            }
            return Result<bool>.Success(true);
        }
    }
}
