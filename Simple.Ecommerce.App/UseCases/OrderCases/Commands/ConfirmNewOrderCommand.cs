using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
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
        private readonly IConfirmedNewOrderUnitOfWork _confirmNewOrderUoW;
        private readonly ICardService _cardService;
        private readonly ICryptographyService _cryptographyService;
        private readonly ProcessConfirmedNewOrderCommandHandler _processConfirmedNewOrderCommandHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ConfirmNewOrderCommand(
            IConfirmedNewOrderUnitOfWork confirmNewOrderUoW,
            ICardService cardService,
            ICryptographyService cryptographyService,
            ProcessConfirmedNewOrderCommandHandler processConfirmedNewOrderCommandHandler,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _confirmNewOrderUoW = confirmNewOrderUoW;
            _cardService = cardService;
            _cryptographyService = cryptographyService;
            _processConfirmedNewOrderCommandHandler = processConfirmedNewOrderCommandHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderCompleteDTO>> Execute(OrderCompleteRequest request)
        {
            await _confirmNewOrderUoW.BeginTransaction();
            try
            {
                var orderRequest = new OrderRequest(
                    request.UserId,
                    request.OrderType,
                    request.Address,
                    request.PaymentInformation,
                    DiscountId: request.DiscountId,
                    Id: request.Id
                );
                var getOrderWithDiscount = await IncludeOrder(orderRequest);
                if (getOrderWithDiscount.IsFailure)
                {
                    throw new ResultException(getOrderWithDiscount.Errors!);
                }
                var (order, userName, orderDiscount) = getOrderWithDiscount.GetValue();

                var orderItemsRequest = new OrderItemsRequest(
                    order.Id,
                    request.OrderItems.Select(item => new OrderItemRequest(
                        item.Quantity,
                        item.ProductId,
                        order.Id,
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
                    _confirmNewOrderUoW.OrderItems.Detach(item.Item1);
                }

                var processNewCommand = new ProcessConfirmedNewOrderCommand(order, userName, orderDiscount, orderItems);
                var processedResult = await _processConfirmedNewOrderCommandHandler.Handle(processNewCommand);
                if (processedResult.IsFailure)
                {
                    throw new ResultException(processedResult.Errors!);
                }

                var getCompleteOrder = await _confirmNewOrderUoW.Orders.GetCompleteOrder(order.Id);
                if (getCompleteOrder.IsFailure)
                {
                    throw new ResultException(getCompleteOrder.Errors!);
                }

                await _confirmNewOrderUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<Order>();
                    _cacheHandler.SetItemStale<OrderItem>();
                }

                return Result<OrderCompleteDTO>.Success(getCompleteOrder.GetValue());
            }
            catch (ResultException rex) 
            {
                await _confirmNewOrderUoW.Rollback();
                return Result<OrderCompleteDTO>.Failure(rex.Errors!);
            }
            catch (Exception ex)
            {
                await _confirmNewOrderUoW.Rollback();
                return Result<OrderCompleteDTO>.Failure(new List<Error>{ new("ConfirmCompleteOrderCommand.Unknown", ex.Message) });
            }
        }

        private async Task<Result<List<(OrderItem, string, Discount?)>>> IncludeOrderItems(OrderItemsRequest request)
        {
            List<OrderItemDiscountInfoDTO> orderItemsDiscountInfo = new();

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
                return Result<List<(OrderItem, string, Discount?)>>.Failure(errors);
            }
            var getProducts = await _confirmNewOrderUoW.Products.GetProductsByIds(productIds);
            if (getProducts.IsFailure)
            {
                return Result<List<(OrderItem, string, Discount?)>>.Failure(getProducts.Errors!);
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
                return Result<List<(OrderItem, string, Discount?)>>.Failure(errors);
            }
            var productsMap = products.ToDictionary(p => p.Id);

            // Pegando todos os produtosdescontos do pedido
            var productDiscountIds = request.OrderItems.Where(item => item.ProductDiscountId is not null).Select(item => item.ProductDiscountId!.Value).ToList();
            var getProductDiscounts = await _confirmNewOrderUoW.ProductDiscounts.GetProductDiscountsByIds(productDiscountIds);
            if (getProductDiscounts.IsFailure)
            {
                return Result<List<(OrderItem, string, Discount?)>>.Failure(getProductDiscounts.Errors!);
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
                return Result<List<(OrderItem, string, Discount?)>>.Failure(errors);
            }
            var productDiscountsMap = productDiscounts.ToDictionary(p => p.Id);

            // Pegando todos os descontos do pedido
            var discountIds = productDiscounts.Select(item => item.DiscountId).ToList();
            var getDiscounts = await _confirmNewOrderUoW.Discounts.GetDiscountsByIds(discountIds);
            if (getDiscounts.IsFailure)
            {
                return Result<List<(OrderItem, string, Discount?)>>.Failure(getDiscounts.Errors!);
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
                return Result<List<(OrderItem, string, Discount?)>>.Failure(errors);
            }
            var discountsMap = discounts.ToDictionary(d => d.Id);

            List<(OrderItem, string, Discount?)> orderItemsNamesDiscounts = new();
            foreach (var orderItemRequest in request.OrderItems)
            {
                Product product = productsMap[orderItemRequest.ProductId];
                Discount? discount = null;
                if (orderItemRequest.ProductDiscountId is not null)
                {
                    var productDiscount = productDiscountsMap[orderItemRequest.ProductDiscountId.Value];
                    if (productDiscount.ProductId != product.Id)
                    {
                        return Result<List<(OrderItem, string, Discount?)>>.Failure(new List<Error>{ new("ConfirmNewOrderCommand.Conflict.ProductId", $"O desconto para produto {productDiscount.Id} é para o produto {productDiscount.ProductId} não para o produto {product.Id}!") });
                    }

                    discount = discountsMap[productDiscount.DiscountId];
                    var productValidation = await ProductDiscountValidation.Validate(_confirmNewOrderUoW.DiscountBundleItems, discount, product, orderItemsDiscountInfo, "ConfirmNewOrderCommand", product.Id);
                    if (productValidation.IsFailure)
                    {
                        return Result<List<(OrderItem, string, Discount?)>>.Failure(productValidation.Errors!);
                    }
                }

                var instance = new OrderItem().Create(
                    0,
                    product.Price,
                    orderItemRequest.Quantity,
                    orderItemRequest.ProductId,
                    request.OrderId,
                    discount is null ? null : discount.Id
                );
                if (instance.IsFailure)
                {
                    return Result<List<(OrderItem, string, Discount?) >>.Failure(instance.Errors!);
                }

                var createResult = await _confirmNewOrderUoW.OrderItems.Create(instance.GetValue());
                if (createResult.IsFailure)
                {
                    return Result<List<(OrderItem, string, Discount?) >>.Failure(createResult.Errors!);
                }
                var orderItem = createResult.GetValue();

                orderItemsDiscountInfo.Add(new OrderItemDiscountInfoDTO(
                    orderItem.OrderId,
                    orderItem.ProductId,
                    orderItem.DiscountId,
                    discount is null ? null : discount.DiscountType
                ));

                orderItemsNamesDiscounts.Add((orderItem, product.Name, discount));
            }

            foreach (var product in products)
                _confirmNewOrderUoW.Products.Detach(product);
            foreach (var productDiscount in productDiscounts)
                _confirmNewOrderUoW.ProductDiscounts.Detach(productDiscount);
            foreach (var discount in discounts)
                _confirmNewOrderUoW.Discounts.Detach(discount);

            return Result<List<(OrderItem, string, Discount?)>>.Success(orderItemsNamesDiscounts);
        }

        private async Task<Result<(Order, string, Discount?)>> IncludeOrder(OrderRequest request)
        {
            var getOrder = await _confirmNewOrderUoW.Orders.Get(request.Id);
            if (getOrder.IsSuccess)
            {
                return Result<(Order, string, Discount?)>.Failure(new List<Error>{ new("ConfirmCompleteOrderCommand.AlreadyExists", "O pedido já existe!") });
            }

            var getUser = await _confirmNewOrderUoW.Users.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<(Order, string, Discount?)>.Failure(getUser.Errors!);
            }

            var getDiscount = request.DiscountId is null ? null : await _confirmNewOrderUoW.Discounts.Get(request.DiscountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                {
                    return Result<(Order, string, Discount?)>.Failure(getDiscount.Errors!);
                }
                List<Error> errors = new();
                var discount = getDiscount.GetValue();

                var simpleValidation = SimpleDiscountValidation.Validate(discount, DiscountScope.Order, "ConfirmCompleteOrderCommand", null);
                if (simpleValidation.IsFailure)
                {
                    errors.AddRange(simpleValidation.Errors!);
                }

                if (errors.Count != 0)
                {
                    return Result<(Order, string, Discount?)>.Failure(errors);
                }           
            }

            var address = new Address(
                request.Address.Number,
                request.Address.Street,
                request.Address.Neighbourhood,
                request.Address.City,
                request.Address.Country,
                request.Address.Complement,
                request.Address.CEP
            );
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
                            return Result<(Order, string, Discount?)>.Failure(isValid.Errors!);
                        
                        var cardFlagResult = _cardService.GetCardFlag(request.PaymentInformation.PaymentKey!);
                        if (cardFlagResult.IsFailure)
                            return Result<(Order, string, Discount?)>.Failure(cardFlagResult.Errors!);
                        cardFlag = cardFlagResult.GetValue();

                        var encryptedKeyResult = _cryptographyService.Encrypt(request.PaymentInformation.PaymentKey!);
                        if (!encryptedKeyResult.IsFailure)
                            return Result<(Order, string, Discount?)>.Failure(encryptedKeyResult.Errors!);
                        encryptedKey = encryptedKeyResult.GetValue();
                        last4Digits = request.PaymentInformation.PaymentKey![^4..];
                        break;
                    default:
                        return Result<(Order, string, Discount?)>.Failure(new List<Error> { new("CreateOrderCommand.InvalidPaymentMethod", "O método de pagamento do pedido não válido.") });
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
                address,
                null,
                null,
                false,
                "Created",
                request.DiscountId,
                OrderLock.Unlock,
                paymentInformation
            );
            if (instance.IsFailure)
            {
                return Result<(Order, string, Discount?)>.Failure(instance.Errors!);
            }

            var createResult = await _confirmNewOrderUoW.Orders.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<(Order, string, Discount?)>.Failure(createResult.Errors!);
            }
            var order = createResult.GetValue();

            _confirmNewOrderUoW.Users.Detach(getUser.GetValue());
            if (getDiscount is not null)
                _confirmNewOrderUoW.Discounts.Detach(getDiscount.GetValue());

            return Result<(Order, string, Discount?)>.Success((order, getUser.GetValue().Name, getDiscount is null ? null : getDiscount.GetValue()));
        }
    }
}
