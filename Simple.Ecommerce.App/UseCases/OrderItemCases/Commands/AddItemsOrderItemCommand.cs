using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class AddItemsOrderItemCommand : IAddItemsOrderItemCommand
    {
        private readonly IAddItemsOrderUnitOfWork _addItemsOrderUoW;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddItemsOrderItemCommand(
            IAddItemsOrderUnitOfWork addItemsOrderUoW,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _addItemsOrderUoW = addItemsOrderUoW;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderItemsResponse>> Execute(OrderItemsRequest request)
        {
            await _addItemsOrderUoW.BeginTransaction();
            try
            {
                var getOrder = await _addItemsOrderUoW.Orders.Get(request.OrderId);
                if (getOrder.IsFailure)
                {
                    throw new ResultException(getOrder.Errors!);
                }
                var order = getOrder.GetValue();

                var getOrderItemsDiscountInfo = await _addItemsOrderUoW.OrderItems.GetOrdemItemsDiscountInfo(order.Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    throw new ResultException(getOrderItemsDiscountInfo.Errors!);
                }
                var orderItemsDiscountInfo = getOrderItemsDiscountInfo.GetValue();
                
                OrderItemsResponse response = new OrderItemsResponse(new List<OrderItemResponse>());
                foreach (var orderItemRequest in request.OrderItems)
                {
                    if (request.OrderId != order.Id)
                    {
                        throw new ResultException(new Error("AddItemsOrderItemCommand.NotRelated.OrderId", "Um dos itens é de um pedido diferente!"));
                    }

                    var getProduct = await _addItemsOrderUoW.Products.Get(orderItemRequest.ProductId);
                    if (getProduct.IsFailure)
                    {
                        throw new ResultException(getProduct.Errors!);
                    }
                    var product = getProduct.GetValue();

                    var getDiscount = orderItemRequest.DiscountId is null ? null : await _addItemsOrderUoW.Discounts.Get(orderItemRequest.DiscountId.Value);
                    if (getDiscount is not null)
                    {
                        if (getDiscount.IsFailure)
                        {
                            throw new ResultException(getDiscount.Errors!);
                        }

                        var validateResult = await ValidateProductDiscount(getDiscount.GetValue(), product, orderItemsDiscountInfo);
                        if (validateResult.IsFailure)
                        {
                            throw new ResultException(validateResult.Errors!);
                        }
                    }

                    OrderItemResponse itemResponse = new OrderItemResponse(0, 0, 0, 0, 0, null);

                    var getOrderItem = await _addItemsOrderUoW.OrderItems.GetByOrderIdAndProductId(order.Id, product.Id);
                    if (getOrderItem.IsSuccess)
                    {
                        var orderItem = getOrderItem.GetValue();
                        orderItem.Update(orderItemRequest.Quantity, product.Price, orderItemRequest.DiscountId, orderItemRequest.Override);
                        if (orderItem.Validate() is { IsFailure: true } result)
                        {
                            throw new ResultException(result.Errors!);
                        }

                        var updateResult = await _addItemsOrderUoW.OrderItems.Update(orderItem, true);
                        if (updateResult.IsFailure)
                        {
                            throw new ResultException(updateResult.Errors!);
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
                            throw new ResultException(instance.Errors!);
                        }

                        var createResult = await _addItemsOrderUoW.OrderItems.Create(instance.GetValue(), true);
                        if (createResult.IsFailure)
                        {
                            throw new ResultException(createResult.Errors!);
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

                await _addItemsOrderUoW.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<OrderItem>();

                return Result<OrderItemsResponse>.Success(response);
            }
            catch (ResultException rex)
            {
                await _addItemsOrderUoW.Rollback();
                return Result<OrderItemsResponse>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _addItemsOrderUoW.Rollback();
                return Result<OrderItemsResponse>.Failure(new List<Error> { new("AddItemsOrderItemCommand.Unknown", ex.Message) });
            }
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
                var getProductIdsOfBundle = await _addItemsOrderUoW.DiscountBundleItems.GetProductIdsByDiscountId(discount.Id);
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
                var getProductBundles = await _addItemsOrderUoW.DiscountBundleItems.GetByProductId(product.Id);
                if (getProductBundles.IsFailure)
                {
                    return Result<bool>.Failure(getProductBundles.Errors!);
                }

                foreach (var bundle in getProductBundles.GetValue())
                {
                    var getProductsIdsOfBundle = await _addItemsOrderUoW.DiscountBundleItems.GetProductIdsByDiscountId(bundle.DiscountId);
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
