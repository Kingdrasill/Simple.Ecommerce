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
    public class ChangeDiscountOrderItemCommand : IChangeDiscountOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangeDiscountOrderItemCommand(
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

        public async Task<Result<bool>> Execute(OrderItemDiscountRequest request)
        {
            var getOrder = await _orderRepository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<bool>.Failure(getOrder.Errors!);
            }

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<bool>.Failure(getProduct.Errors!);
            }

            var getOrderItem = await _repository.GetByOrderIdAndProductId(request.OrderId, request.ProductId);
            if (getOrderItem.IsFailure)
            {
                return Result<bool>.Failure(getOrderItem.Errors!);
            }
            var orderItem = getOrderItem.GetValue();

            var getDiscount = request.DiscountId is null ? null : await _discountRepository.Get(request.DiscountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                    return Result<bool>.Failure(getDiscount.Errors!);

                var getOrderItemsDiscountInfo = await _repository.GetOrdemItemsDiscountInfo(getOrder.GetValue().Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    return Result<bool>.Failure(getOrderItemsDiscountInfo.Errors!);
                }

                var validateResult = await ValidateProductDiscount(getDiscount.GetValue(), getProduct.GetValue(), getOrderItemsDiscountInfo.GetValue());
                if (validateResult.IsFailure)
                {
                    return Result<bool>.Failure(validateResult.Errors!);
                }
            }

            orderItem.UpdateDiscountId(request.DiscountId);
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
