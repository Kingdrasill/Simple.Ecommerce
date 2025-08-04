using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ConfirmNewOrderCommand : IConfirmNewOrderCommand
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;
        private readonly ICardService _cardService;
        private readonly ICryptographyService _cryptographyService;
        private readonly ProcessConfirmedNewOrderCommandHandler _processConfirmedNewOrderCommandHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ConfirmNewOrderCommand(
            IConfirmOrderUnitOfWork confirmOrderUoW,
            ICardService cardService,
            ICryptographyService cryptographyService,
            ProcessConfirmedNewOrderCommandHandler processConfirmedNewOrderCommandHandler,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _confirmOrderUoW = confirmOrderUoW;
            _cardService = cardService;
            _cryptographyService = cryptographyService;
            _processConfirmedNewOrderCommandHandler = processConfirmedNewOrderCommandHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderCompleteDTO>> Execute(OrderCompleteRequest request)
        {
            await _confirmOrderUoW.BeginTransaction();
            try
            {
                var orderRequest = new OrderRequest(
                    request.UserId,
                    request.OrderType,
                    request.Address,
                    request.PaymentInformation,
                    CouponCode: request.CouponCode,
                    DiscountId: request.DiscountId,
                    Id: request.Id
                );
                var getOrderWithDiscount = await IncludeOrder(orderRequest);
                if (getOrderWithDiscount.IsFailure)
                {
                    throw new ResultException(getOrderWithDiscount.Errors!);
                }
                var (order, userName, orderCoupon, orderDiscount) = getOrderWithDiscount.GetValue();

                var orderItemsRequest = new OrderItemsRequest(
                    order.Id,
                    request.OrderItems.Select(item => new OrderItemRequest(
                        item.Quantity,
                        item.ProductId,
                        order.Id,
                        item.CouponCode,
                        item.ProductDiscountId
                    )).ToList()
                );
                var getOrderItems = await IncludeOrderItems(orderItemsRequest);
                if (getOrderItems.IsFailure)
                {
                    throw new ResultException(getOrderItems.Errors!);
                }
                var orderItems = getOrderItems.GetValue();

                foreach (var item in orderItems)
                {
                    _confirmOrderUoW.OrderItems.Detach(item.Item1);
                }

                var processNewCommand = new ProcessConfirmedNewOrderCommand(order, userName, orderCoupon, orderDiscount, orderItems);
                var processedResult = await _processConfirmedNewOrderCommandHandler.Handle(processNewCommand);
                if (processedResult.IsFailure)
                {
                    throw new ResultException(processedResult.Errors!);
                }

                var getCompleteOrder = await _confirmOrderUoW.Orders.GetCompleteOrder(order.Id);
                if (getCompleteOrder.IsFailure)
                {
                    throw new ResultException(getCompleteOrder.Errors!);
                }

                await _confirmOrderUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<Order>();
                    _cacheHandler.SetItemStale<OrderItem>();
                }

                return Result<OrderCompleteDTO>.Success(getCompleteOrder.GetValue());
            }
            catch (ResultException rex) 
            {
                await _confirmOrderUoW.Rollback();
                return Result<OrderCompleteDTO>.Failure(rex.Errors!);
            }
            catch (Exception ex)
            {
                await _confirmOrderUoW.Rollback();
                return Result<OrderCompleteDTO>.Failure(new List<Error>{ new("ConfirmCompleteOrderCommand.Unknown", ex.Message) });
            }
        }

        private async Task<Result<List<(OrderItem, string, Coupon?, Discount?)>>> IncludeOrderItems(OrderItemsRequest request)
        {
            // Pegando todos os produtos do pedido
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
                    errors.Add(new("ConfirmNewOrderCommand.Duplicate.OrderItem", $"O pedido não pode ter items duplicados para o produto {duplicate}!"));
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }
            var getProducts = await _confirmOrderUoW.Products.GetProductsByIds(productIds);
            if (getProducts.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getProducts.Errors!);
            }
            var products = getProducts.GetValue();
            if (productIds.Count != products.Count)
            {
                List<Error> errors = new List<Error>();
                foreach (var productId in productIds)
                {
                    if (!products.Any(p => p.Id == productId))
                    {
                        errors.Add(new("ConfirmNewOrderCommand.NotFound", $"O produto {productId} não foi encontrado!"));
                    }
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }
            var productsMap = products.ToDictionary(p => p.Id);

            // Pegando todos os cupons do pedido
            var couponCodes = request.OrderItems.Where(item => item.CouponCode is not null).Select(item => item.CouponCode!).ToList();
            var getCoupons = await _confirmOrderUoW.Coupons.ListByCodes(couponCodes);
            if (getCoupons.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getCoupons.Errors!);
            }
            var coupons = getCoupons.GetValue();
            if (couponCodes.Count != coupons.Count)
            {
                List<Error> errors = new();
                foreach (var couponCode in couponCodes)
                {
                    if (!coupons.Any(c => c.Code == couponCode))
                    {
                        errors.Add(new("ConfirmNewOrderCommand.NotFound", $"O cupom {couponCode} não foi encontrado!"));
                    }
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }
            foreach (var coupon in coupons)
            {
                if (coupon.IsUsed)
                {
                    return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(new List<Error> { new("ConfirmCompleteOrderCommand.AlreadyUsed", $"O cupom {coupon.Code} já foi usado!") });
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
            var getProductDiscountFromCoupons = await _confirmOrderUoW.ProductDiscounts
                .GetByProductIdsDiscountIds(productAndDiscountForCoupon.Select(i => (i.ProductId, i.DiscountId)).ToList());
            if (getProductDiscountFromCoupons.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getProductDiscountFromCoupons.Errors!);
            }
            var productDiscountFromCoupons = getProductDiscountFromCoupons.GetValue();
            if (productAndDiscountForCoupon.Count != productDiscountFromCoupons.Count)
            {
                List<Error> errors = new();
                foreach (var productAndDiscount in productAndDiscountForCoupon)
                {
                    if (!productDiscountFromCoupons.Any(c => c.ProductId == productAndDiscount.ProductId && c.DiscountId == productAndDiscount.DiscountId))
                    {
                        errors.Add(new("ConfirmNewOrderCommand.NoRelation", $"O desconto do cupom {productAndDiscount.CouponCode} não é para o produto {productAndDiscount.ProductId}!"));
                    }
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }

            // Pegando os descontos dos cupons
            var couponDiscountIds = coupons.Select(c => c.DiscountId).ToList();
            var getCouponDiscounts = await _confirmOrderUoW.Discounts.GetDiscountsByIds(couponDiscountIds);
            if (getCouponDiscounts.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getCouponDiscounts.Errors!);
            }
            var couponDiscounts = getCouponDiscounts.GetValue();
            var couponDiscountsMap = couponDiscounts.ToDictionary(d => d.Id);

            // Pegando todos os descontos de produto do pedido
            var productDiscountIds = request.OrderItems.Where(item => item.ProductDiscountId is not null).Select(item => item.ProductDiscountId!.Value).ToList();
            var getProductDiscounts = await _confirmOrderUoW.ProductDiscounts.GetProductDiscountsByIds(productDiscountIds);
            if (getProductDiscounts.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getProductDiscounts.Errors!);
            }
            var productDiscounts = getProductDiscounts.GetValue();
            if (productDiscountIds.Count != productDiscounts.Count)
            {
                List<Error> errors = new List<Error>();
                foreach (var productDiscountId in productDiscountIds)
                {
                    if (!productDiscounts.Any(p => p.Id == productDiscountId))
                    {
                        errors.Add(new("ConfirmNewOrderCommand.NotFound", $"O desconto para o produto {productDiscountId} não foi encontrado!"));
                    }
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }
            var productDiscountsMap = productDiscounts.ToDictionary(p => p.Id);

            // Pegando todos os descontos do pedido
            var discountIds = productDiscounts.Select(item => item.DiscountId).ToList();
            var getDiscounts = await _confirmOrderUoW.Discounts.GetDiscountsByIds(discountIds);
            if (getDiscounts.IsFailure)
            {
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(getDiscounts.Errors!);
            }
            var discounts = getDiscounts.GetValue();
            if (discountIds.Count != discounts.Count)
            {
                List<Error> errors = new List<Error>();
                foreach (var discountId in discountIds)
                {
                    if (!discounts.Any(p => p.Id == discountId))
                    {
                        errors.Add(new("ConfirmNewOrderCommand.NotFound", $"O desconto {discountId} não foi encontrado!"));
                    }
                }
                return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(errors);
            }
            var discountsMap = discounts.ToDictionary(d => d.Id);

            List<(OrderItem, string, Coupon?, Discount?)> orderItemsNamesDiscounts = new();
            List<OrderItemInfoDTO> orderItemsDiscountInfo = new();
            foreach (var orderItemRequest in request.OrderItems)
            {
                Product product = productsMap[orderItemRequest.ProductId];
                Coupon? coupon = null;
                Discount? discount = null;
                if (orderItemRequest.CouponCode is not null)
                {
                    coupon = couponsMap[orderItemRequest.CouponCode];
                    discount = couponDiscountsMap[coupon.DiscountId];

                    if (!productAndDiscountForCouponMap.TryGetValue((product.Id, coupon.DiscountId), out var productDiscount))
                    {
                        return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(
                            new List<Error> { new("ConfirmNewOrderCommand.Conflict.ProductId", $"O cupom {coupon.Code} para o desconto {discount.Id} não é para o produto {product.Id}!") }
                        );
                    }
                }
                else if (orderItemRequest.ProductDiscountId is not null)
                {
                    var productDiscount = productDiscountsMap[orderItemRequest.ProductDiscountId.Value];
                    if (productDiscount.ProductId != product.Id)
                    {
                        return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(new List<Error>{ new("ConfirmNewOrderCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} não é para o produto {product.Id}!") });
                    }
                    discount = discountsMap[productDiscount.DiscountId];
                }

                if (discount is not null)
                {
                    var productValidation = await ProductDiscountValidation.Validate(_confirmOrderUoW.DiscountBundleItems, coupon, discount, product, orderItemsDiscountInfo, "ConfirmNewOrderCommand", product.Id);
                    if (productValidation.IsFailure)
                    {
                        return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Failure(productValidation.Errors!);
                    }
                }

                var instance = new OrderItem().Create(
                    0,
                    product.Price,
                    orderItemRequest.Quantity,
                    orderItemRequest.ProductId,
                    request.OrderId,
                    coupon?.Id,
                    discount?.Id
                );
                if (instance.IsFailure)
                {
                    return Result<List<(OrderItem, string, Coupon?, Discount?) >>.Failure(instance.Errors!);
                }

                var createResult = await _confirmOrderUoW.OrderItems.Create(instance.GetValue());
                if (createResult.IsFailure)
                {
                    return Result<List<(OrderItem, string, Coupon?, Discount?) >>.Failure(createResult.Errors!);
                }
                var orderItem = createResult.GetValue();

                orderItemsDiscountInfo.Add(new OrderItemInfoDTO(
                    orderItem.Id,
                    orderItem.ProductId,
                    discount is null 
                        ? null 
                        : new DiscountInfoDTO(
                            discount.Id,
                            discount.Name,
                            discount.DiscountType
                        ),
                    coupon is null 
                        ? null 
                        : new CouponInfoDTO(
                            coupon.Id,
                            coupon.Code
                        )
                ));

                orderItemsNamesDiscounts.Add((orderItem, product.Name, coupon, discount));
            }

            foreach (var product in products)
                _confirmOrderUoW.Products.Detach(product);
            foreach (var coupon in coupons)
                _confirmOrderUoW.Coupons.Detach(coupon);
            foreach (var productDiscount in productDiscounts)
                _confirmOrderUoW.ProductDiscounts.Detach(productDiscount);
            foreach (var discount in discounts)
                _confirmOrderUoW.Discounts.Detach(discount);

            return Result<List<(OrderItem, string, Coupon?, Discount?)>>.Success(orderItemsNamesDiscounts);
        }

        private async Task<Result<(Order, string, Coupon?, Discount?)>> IncludeOrder(OrderRequest request)
        {
            var getOrder = await _confirmOrderUoW.Orders.Get(request.Id);
            if (getOrder.IsSuccess)
            {
                return Result<(Order, string, Coupon?, Discount?)>.Failure(new List<Error>{ new("ConfirmCompleteOrderCommand.AlreadyExists", "O pedido já existe!") });
            }

            var getUser = await _confirmOrderUoW.Users.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<(Order, string, Coupon?, Discount?)>.Failure(getUser.Errors!);
            }
            var user = getUser.GetValue();

            var couponCode = request.CouponCode;
            var discountId = request.DiscountId;

            Coupon? coupon = null;
            var getCoupon = couponCode is null ? null : await _confirmOrderUoW.Coupons.GetByCode(couponCode);
            if (getCoupon is not null)
            {
                if (getCoupon.IsFailure)
                {
                    return Result<(Order, string, Coupon?, Discount?)>.Failure(getCoupon.Errors!);
                }
                coupon = getCoupon.GetValue();
                
                if (coupon.IsUsed)
                {
                    return Result<(Order, string, Coupon?, Discount?)>.Failure(new List<Error> { new("ConfirmCompleteOrderCommand.AlreadyUsed", $"O cupom {couponCode} já foi usado!") });
                }

                discountId = coupon.DiscountId;
            }

            Discount? discount = null;
            var getDiscount = discountId is null ? null : await _confirmOrderUoW.Discounts.Get(discountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                {
                    return Result<(Order, string, Coupon?, Discount?)>.Failure(getDiscount.Errors!);
                }
                discount = getDiscount.GetValue();

                var simpleValidation = SimpleDiscountValidation.Validate(discount, DiscountScope.Order, "ConfirmCompleteOrderCommand", null);
                if (simpleValidation.IsFailure)
                {
                    return Result<(Order, string, Coupon?, Discount?)>.Failure(simpleValidation.Errors!);
                }         
            }

            PaymentInformation? paymentInformation = null;
            if(request.PaymentInformation is not null)
            {
                CardFlag? cardFlag = null;
                string? encryptedKey = null;
                string? last4Digits = null;
                switch (request.PaymentInformation.PaymentMethod)
                {
                    case PaymentMethod.Cash:
                        break;
                    case PaymentMethod.Pix:
                        encryptedKey = request.PaymentInformation.PaymentKey!;
                        break;
                    case (PaymentMethod.CreditCard or PaymentMethod.DebitCard):
                        var isValid = _cardService.IsValidCardNumber(request.PaymentInformation.PaymentKey!);
                        if (isValid.IsFailure)
                            return Result<(Order, string, Coupon?, Discount?)>.Failure(isValid.Errors!);
                        
                        var cardFlagResult = _cardService.GetCardFlag(request.PaymentInformation.PaymentKey!);
                        if (cardFlagResult.IsFailure)
                            return Result<(Order, string, Coupon?, Discount?)>.Failure(cardFlagResult.Errors!);
                        cardFlag = cardFlagResult.GetValue();

                        var encryptedKeyResult = _cryptographyService.Encrypt(request.PaymentInformation.PaymentKey!);
                        if (!encryptedKeyResult.IsFailure)
                            return Result<(Order, string, Coupon?, Discount?)>.Failure(encryptedKeyResult.Errors!);
                        encryptedKey = encryptedKeyResult.GetValue();
                        last4Digits = request.PaymentInformation.PaymentKey![^4..];
                        break;
                    default:
                        return Result<(Order, string, Coupon?, Discount?)>.Failure(new List<Error> { new("CreateOrderCommand.InvalidPaymentMethod", "O método de pagamento do pedido não válido.") });
                }
                paymentInformation = new PaymentInformation(
                    request.PaymentInformation.PaymentMethod,
                    request.PaymentInformation.PaymentName,
                    encryptedKey,
                    request.PaymentInformation.ExpirationMonth,
                    request.PaymentInformation.ExpirationYear,
                    cardFlag,
                    last4Digits
                );
            }
            var instance = new Order().Create(
                request.Id,
                request.UserId,
                request.OrderType,
                new Address(
                    request.Address.Number,
                    request.Address.Street,
                    request.Address.Neighbourhood,
                    request.Address.City,
                    request.Address.Country,
                    request.Address.Complement,
                    request.Address.CEP
                ),
                null,
                null,
                false,
                "Created",
                coupon is null ? null : coupon.Id,
                discount is null ? null : discount.Id,
                OrderLock.Unlock,
                paymentInformation
            );
            if (instance.IsFailure)
            {
                return Result<(Order, string, Coupon?, Discount?)>.Failure(instance.Errors!);
            }

            var createResult = await _confirmOrderUoW.Orders.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<(Order, string, Coupon?, Discount?)>.Failure(createResult.Errors!);
            }
            var order = createResult.GetValue();

            _confirmOrderUoW.Users.Detach(user);
            if (coupon is not null)
                _confirmOrderUoW.Coupons.Detach(coupon);
            if (discount is not null)
                _confirmOrderUoW.Discounts.Detach(discount);

            return Result<(Order, string, Coupon?, Discount?)>.Success((order, user.Name, coupon, discount));
        }
    }
}
