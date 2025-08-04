using Microsoft.EntityFrameworkCore.Internal;
using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
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

                if (order.OrderLock is not OrderLock.Unlock)
                {
                    return Result<OrderItemsResponse>.Failure(new List<Error> { new("AddItemsOrderItemCommand.OrderLocked", "Não é possível mudar os dados do pedido!") });
                }

                var getOrderItemsDiscountInfo = await _addItemsOrderUoW.OrderItems.ListByOrderIdOrderItemInfoDTO(order.Id);
                if (getOrderItemsDiscountInfo.IsFailure)
                {
                    throw new ResultException(getOrderItemsDiscountInfo.Errors!);
                }
                var orderItemsDiscountInfo = getOrderItemsDiscountInfo.GetValue();

                // Buscando todos os produtos passados
                var productIds = request.OrderItems.Select(item => item.ProductId).ToList();
                var duplicates = productIds
                    .GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (duplicates.Count != 0)
                {
                    List<Error> errors = new List<Error>();
                    foreach (var duplicate in duplicates)
                    {
                        errors.Add(new("AddItemsOrderItemCommand.Duplicate.OrderItem", $"Não pode ter items duplicados para o produto {duplicate}!"));
                    }
                    throw new ResultException(errors);
                }
                var getProducts = await _addItemsOrderUoW.Products.GetProductsByIds(productIds);
                if (getProducts.IsFailure)
                {
                    throw new ResultException(getProducts.Errors!);
                }
                var products = getProducts.GetValue();
                if (productIds.Count != products.Count)
                {
                    List<Error> errors = new List<Error>();
                    foreach (var productId in productIds)
                    {
                        if (!products.Any(p => p.Id == productId))
                        {
                            errors.Add(new("AddItemsOrderItemCommand.NotFound", $"O produto {productId} não foi encontrado!"));
                        }
                    }
                    throw new ResultException(errors);
                }
                var productsMap = products.ToDictionary(p => p.Id);

                // Pegando todos os cupons passados
                var couponCodes = request.OrderItems.Where(item => item.CouponCode is not null).Select(item => item.CouponCode!).ToList();
                var getCoupons = await _addItemsOrderUoW.Coupons.ListByCodes(couponCodes);
                if (getCoupons.IsFailure)
                {
                    throw new ResultException(getCoupons.Errors!);
                }
                var coupons = getCoupons.GetValue();
                if (couponCodes.Count != coupons.Count)
                {
                    List<Error> errors = new();
                    foreach (var couponCode in couponCodes)
                    {
                        if (!coupons.Any(c => c.Code == couponCode))
                        {
                            errors.Add(new("AddItemsOrderItemCommand.NotFound", $"O cupom {couponCode} não foi encontrado!"));
                        }
                    }
                    throw new ResultException(errors);
                }
                foreach (var coupon in coupons)
                {
                    if (coupon.IsUsed)
                    {
                        throw new ResultException(new Error("AddItemsOrderItemCommand.AlreadyUsed", $"O cupom {coupon.Code} já foi usado!"));
                    }
                }
                var couponsMap = coupons.ToDictionary(c => c.Code);

                // Verificando se os produtos com cupons tem o desconto para produto aplicado
                var productAndDiscountForCoupon = request.OrderItems
                    .Where(oi => oi.CouponCode is not null)
                    .Select(oi =>
                    {
                        var coupon = couponsMap[oi.CouponCode!];
                        return (ProductId: oi.ProductId, DiscountId: coupon.DiscountId, CouponCode: coupon.Code);
                    })
                    .Distinct()
                    .ToList();
                var productAndDiscountForCouponMap = productAndDiscountForCoupon.ToDictionary(pdc => (pdc.ProductId, pdc.DiscountId));
                var getProductDiscountFromCoupons = await _addItemsOrderUoW.ProductDiscounts
                    .GetByProductIdsDiscountIds(productAndDiscountForCoupon.Select(i => (i.ProductId, i.DiscountId)).ToList());
                if (getProductDiscountFromCoupons.IsFailure)
                {
                    throw new ResultException(getProductDiscountFromCoupons.Errors!);
                }
                var productDiscountFromCoupons = getProductDiscountFromCoupons.GetValue();
                if (productAndDiscountForCoupon.Count != productDiscountFromCoupons.Count)
                {
                    List<Error> errors = new();
                    foreach (var productAndDiscount in productAndDiscountForCoupon)
                    {
                        if (!productDiscountFromCoupons.Any(c => c.ProductId == productAndDiscount.ProductId && c.DiscountId == productAndDiscount.DiscountId))
                        {
                            errors.Add(new("AddItemsOrderItemCommand.NoRelation", $"O desconto do cupom {productAndDiscount.CouponCode} não é para o produto {productAndDiscount.ProductId}!"));
                        }
                    }
                    throw new ResultException(errors);
                }

                // Pegando os descontos dos cupons
                var couponDiscountIds = coupons.Select(c => c.DiscountId).ToList();
                var getCouponDiscounts = await _addItemsOrderUoW.Discounts.GetDiscountsByIds(couponDiscountIds);
                if (getCouponDiscounts.IsFailure)
                {
                    throw new ResultException(getCouponDiscounts.Errors!);
                }
                var couponDiscounts = getCouponDiscounts.GetValue();
                var couponDiscountsMap = couponDiscounts.ToDictionary(d => d.Id);

                // Buscando todos os descontos para produtos passados
                var productDiscountIds = request.OrderItems.Where(item => item.ProductDiscountId is not null).Select(item => item.ProductDiscountId!.Value).ToList();
                var getProductDiscounts = await _addItemsOrderUoW.ProductDiscounts.GetProductDiscountsByIds(productDiscountIds);
                if (getProductDiscounts.IsFailure)
                {
                    throw new ResultException(getProductDiscounts.Errors!);
                }
                var productDiscounts = getProductDiscounts.GetValue();
                if (productDiscountIds.Count != productDiscounts.Count)
                {
                    List<Error> errors = new List<Error>();
                    foreach (var productDiscountId in productDiscountIds)
                    {
                        if (!productDiscounts.Any(p => p.Id == productDiscountId))
                        {
                            errors.Add(new("AddItemsOrderItemCommand.NotFound", $"O desconto para o produto {productDiscountId} não foi encontrado!"));
                        }
                    }
                    throw new ResultException(errors);
                }
                var productDiscountsMap = productDiscounts.ToDictionary(p => p.Id);

                // Buscando todos os descontos passados
                var discountIds = productDiscounts.Select(item => item.DiscountId).ToList();
                var getDiscounts = await _addItemsOrderUoW.Discounts.GetDiscountsByIds(discountIds);
                if (getDiscounts.IsFailure)
                {
                    throw new ResultException(getDiscounts.Errors!);
                }
                var discounts = getDiscounts.GetValue();
                if (discountIds.Count != discounts.Count)
                {
                    List<Error> errors = new List<Error>();
                    foreach (var discountId in discountIds)
                    {
                        if (!discounts.Any(p => p.Id == discountId))
                        {
                            errors.Add(new("AddItemsOrderItemCommand.NotFound", $"O desconto {discountId} não foi encontrado!"));
                        }
                    }
                    throw new ResultException(errors);
                }
                var discountsMap = discounts.ToDictionary(d => d.Id);

                OrderItemsResponse response = new OrderItemsResponse(new List<OrderItemResponse>());
                foreach (var orderItemRequest in request.OrderItems)
                {
                    if (request.OrderId != order.Id)
                    {
                        throw new ResultException(new Error("AddItemsOrderItemCommand.NotRelated.OrderId", "Um dos itens é de um pedido diferente!"));
                    }

                    Product product = productsMap[orderItemRequest.ProductId];
                    Coupon? coupon = null;
                    Discount? discount = null;
                    if (orderItemRequest.CouponCode is not null)
                    {
                        coupon = couponsMap[orderItemRequest.CouponCode];
                        discount = couponDiscountsMap[coupon.DiscountId];

                        if (!productAndDiscountForCouponMap.TryGetValue((product.Id, discount.Id), out var productDiscount))
                        {
                            throw new ResultException(new Error("AddItemsOrderItemCommand.Conflict.ProductId", $"O cupom {coupon.Code} para o desconto {discount.Id} não é para o produto {product.Id}!"));
                        }
                    }
                    else if (orderItemRequest.ProductDiscountId is not null)
                    {
                        var productDiscount = productDiscountsMap[orderItemRequest.ProductDiscountId.Value];
                        if (productDiscount.ProductId != orderItemRequest.ProductId)
                        {
                            throw new ResultException(new Error("AddItemsOrderItemCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} é para o produto {productDiscount.ProductId} não para o produto {product.Id}!"));
                        }

                        discount = discountsMap[productDiscount.DiscountId];
                    }

                    if (discount is not null)
                    {
                        var productValidation = await ProductDiscountValidation.Validate(_addItemsOrderUoW.DiscountBundleItems, coupon, discount, product, orderItemsDiscountInfo, "AddItemsOrderItemCommand", product.Id);
                        if (productValidation.IsFailure)
                        {
                            throw new ResultException(productValidation.Errors!);
                        }
                    }

                    OrderItemResponse itemResponse = new OrderItemResponse(0, 0, 0, 0, 0, null);
                    var getOrderItem = await _addItemsOrderUoW.OrderItems.GetByOrderIdAndProductId(order.Id, product.Id);
                    if (getOrderItem.IsSuccess)
                    {
                        var orderItem = getOrderItem.GetValue();
                        orderItem.Update(
                            orderItemRequest.Quantity, 
                            product.Price, 
                            coupon?.Id,
                            discount?.Id, 
                            orderItemRequest.Override
                        );
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
                            coupon?.Id,
                            discount?.Id
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

                    orderItemsDiscountInfo.Add(new OrderItemInfoDTO(
                        itemResponse.Id,
                        itemResponse.ProductId,
                        discount != null ? new DiscountInfoDTO(
                            discount.Id,
                            discount.Name,
                            discount.DiscountType
                        ) : null,
                        coupon != null ? new CouponInfoDTO(
                            coupon.Id,
                            coupon.Code
                        ) : null
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
    }
}
