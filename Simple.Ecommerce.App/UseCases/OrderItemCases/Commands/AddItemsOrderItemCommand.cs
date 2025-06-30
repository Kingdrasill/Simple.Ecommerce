using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class AddItemsOrderItemCommand : IAddItemsOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddItemsOrderItemCommand(
            IOrderItemRepository repository, 
            IOrderRepository orderRepository, 
            IProductRepository productRepository, 
            IDiscountRepository discountRepository,
            IDiscountBundleItemRepository discountBundleItemRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _discountBundleItemRepository = discountBundleItemRepository;
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

            var order = getOrder.GetValue();

            var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(order.Id);
            if (getOrderItemsDiscountInfo.IsFailure)
            {
                return Result<OrderItemsResponse>.Failure(getOrderItemsDiscountInfo.Errors!);
            }

            var orderItemsDiscountInfo = getOrderItemsDiscountInfo.GetValue();
            OrderItemsResponse response = new OrderItemsResponse(new List<OrderItemResponse>());

            foreach (var orderItemRequest in request.OrderItems)
            {
                if (request.OrderId != order.Id)
                {
                    return Result<OrderItemsResponse>.Failure(new List<Error> { new("AddItemsOrderItemCommand.NotRelated.OrderId", "Um dos itens é de um pedido diferente!") });
                }

                var getProduct = await _productRepository.Get(orderItemRequest.ProductId);
                if (getProduct.IsFailure)
                {
                    return Result<OrderItemsResponse>.Failure(getProduct.Errors!);
                }

                var product = getProduct.GetValue();

                var getDiscount = orderItemRequest.DiscountId is null ? null : await _discountRepository.Get(orderItemRequest.DiscountId.Value);
                if (getDiscount is not null)
                {
                    if (getDiscount.IsFailure)
                    {
                        return Result<OrderItemsResponse>.Failure(getDiscount.Errors!);
                    }

                    var validateResult = await ValidateProductDiscount(getDiscount.GetValue(), product, orderItemsDiscountInfo);
                    if (validateResult.IsFailure)
                    {
                        return Result<OrderItemsResponse>.Failure(validateResult.Errors!);
                    }
                }

                OrderItemResponse itemResponse = new OrderItemResponse(0, 0, 0, 0, 0, null);

                var getOrderItem = await _repository.GetByOrderIdAndProductId(order.Id, product.Id);
                if (getOrderItem.IsSuccess)
                {
                    var orderItem = getOrderItem.GetValue();
                    orderItem.Update(orderItemRequest.Quantity, product.Price, orderItemRequest.DiscountId, orderItemRequest.Override);

                    var validateOrderItem = orderItem.Validate();
                    if (validateOrderItem.IsFailure)
                    {
                        return Result<OrderItemsResponse>.Failure(validateOrderItem.Errors!);
                    }

                    var updateResult = await _repository.Update(orderItem);
                    if (updateResult.IsFailure)
                    {
                        return Result<OrderItemsResponse>.Failure(updateResult.Errors!);
                    }

                    var orderItemUpdated = updateResult.GetValue();

                    itemResponse = new OrderItemResponse(
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
                        product.Price,
                        orderItemRequest.Quantity,
                        orderItemRequest.ProductId,
                        orderItemRequest.OrderId,
                        orderItemRequest.DiscountId
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

                    itemResponse = new OrderItemResponse(
                        orderItem.Id,
                        orderItem.Price,
                        orderItem.Quantity,
                        orderItem.ProductId,
                        orderItem.OrderId,
                        orderItem.DiscountId
                    );
                }

                orderItemsDiscountInfo.Add(new OrderItemDiscountInfoDTO(
                    itemResponse.OrderId,
                    itemResponse.ProductId,
                    itemResponse.DiscountId,
                    getDiscount is null ? null : getDiscount.GetValue().DiscountType
                ));

                response.OrderItems.Add(itemResponse);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<OrderItemsResponse>.Success(response);
        }

        private async Task<Result<bool>> ValidateProductDiscount(Discount discount, Product product, List<OrderItemDiscountInfoDTO> orderItemsDiscountInfo)
        {
            List<Error> errors = new();

            if (discount.DiscountScope != DiscountScope.Product)
                errors.Add(new("AddItemsOrderItemCommand.InvalidDiscountScope", "Um dos itens tem um desconto que não é aplicável a produtos!"));

            if (discount.DiscountType == DiscountType.FirstPurchase)
                errors.Add(new("AddItemsOrderItemCommand.InvalidDiscountType", "Um dos itens tem um desconto que não é aplicável a pedidos!"));

            if (discount.ValidFrom > DateTime.UtcNow)
                errors.Add(new("AddItemsOrderItemCommand.DiscountNotValidYet", "Um dos itens tem um desconto que ainda não está válido!"));

            if (discount.ValidTo < DateTime.UtcNow)
                errors.Add(new("AddItemsOrderItemCommand.DiscountExpired", "Um dos itens tem um desconto que já expirou!"));

            if (!discount.IsActive)
                errors.Add(new("AddItemsOrderItemCommand.InactiveDiscount", "Um dos itens tem um desconto que não está ativo!"));

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
                        errors.Add(new("AddItemsOrderItemCommand.ConflictDiscountType", $"Um produto do desconto de pacote {discount.Id} já está utilizando um desconto que não é de pacote!"));
                    else if (bundleItem.DiscountId != discount.Id)
                        errors.Add(new("AddItemsOrderItemCommand.ConflictDiscountBundle", $"Um produto do desconto de pacote {discount.Id} já está utilizando um desconto para outro desconto de pacote!"));
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
                            errors.Add(new("AddItemsOrderItemCommand.ConflictDiscountType", "Um dos produtos do pedido está utilizando o desconto de pacote que este um dos produtos inseridos faz parte mas este está usando outro desconto!"));
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
