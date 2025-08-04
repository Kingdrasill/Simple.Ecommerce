using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class UpdateOrderCommand : IUpdateOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ICardService _cardService;
        private readonly ICryptographyService _cryptographyService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateOrderCommand(
            IOrderRepository repository,
            IUserRepository userRepository,
            IDiscountRepository discountRepository,
            ICouponRepository couponRepository,
            ICardService cardService,
            ICryptographyService cryptographyService,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _discountRepository = discountRepository;
            _couponRepository = couponRepository;
            _cardService = cardService;
            _cryptographyService = cryptographyService;
            _cacheHandler = cacheHandler;
            _useCache = useCache;
        }

        public async Task<Result<OrderResponse>> Execute(OrderRequest request)
        {
            var getOrder = await _repository.Get(request.Id);
            if (getOrder.IsFailure)
            {
                return Result<OrderResponse>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not OrderLock.Unlock)
            {
                return Result<OrderResponse>.Failure(new List<Error> { new("UpdateOrderCommand.OrderLocked", "Não é possível mudar os dados do pedido!") });
            }

            var getUser = await _userRepository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<OrderResponse>.Failure(getUser.Errors!);
            }

            var couponCode = request.CouponCode;
            var discountId = request.DiscountId;
            Coupon? coupon = null;
            Discount? discount = null;

            var getCoupon = couponCode is null ? null : await _couponRepository.GetByCode(couponCode);
            var getDiscount = discountId is null ? null : await _discountRepository.Get(discountId.Value);
            if (getCoupon is not null)
            {
                if (getCoupon.IsFailure)
                {
                    return Result<OrderResponse>.Failure(getCoupon.Errors!);
                }
                coupon = getCoupon.GetValue();

                if (coupon.IsUsed)
                {
                    return Result<OrderResponse>.Failure(new List<Error> { new("UpdateOrderCommand.AlreadyUsed", $"O cupom {coupon.Code} já foi usado!") });
                }

                getDiscount = await _discountRepository.Get(coupon.DiscountId);
            }
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                {
                    return Result<OrderResponse>.Failure(getDiscount.Errors!);
                }
                discount = getDiscount.GetValue();

                var simpleValidation = SimpleDiscountValidation.Validate(discount, DiscountScope.Order, "UpdateOrderCommand", null);
                if (simpleValidation.IsFailure)
                {
                    return Result<OrderResponse>.Failure(simpleValidation.Errors!);
                }
            }

            PaymentInformation? paymentInformation = null;
            if (request.PaymentInformation is not null)
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
                        {
                            return Result<OrderResponse>.Failure(isValid.Errors!);
                        }

                        var cardFlagResult = _cardService.GetCardFlag(request.PaymentInformation.PaymentKey!);
                        if (cardFlagResult.IsFailure)
                        {
                            return Result<OrderResponse>.Failure(cardFlagResult.Errors!);
                        }
                        cardFlag = cardFlagResult.GetValue();

                        var encryptedKeyResult = _cryptographyService.Encrypt(request.PaymentInformation.PaymentKey!);
                        if (!encryptedKeyResult.IsFailure)
                        {
                            return Result<OrderResponse>.Failure(encryptedKeyResult.Errors!);
                        }
                        encryptedKey = encryptedKeyResult.GetValue();
                        last4Digits = request.PaymentInformation.PaymentKey![^4..];
                        break;
                    default:
                        return Result<OrderResponse>.Failure(new List<Error> { new("UpdateOrderCommand.InvalidPaymentMethod", "O método de pagamento passado não válido.") });
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
                request.TotalPrice,
                request.OrderDate,
                order.Confirmation,
                "Altered",
                coupon?.Id,
                discount?.Id,
                order.OrderLock,
                paymentInformation
            );
            if (instance.IsFailure)
            {
                return Result<OrderResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<OrderResponse>.Failure(updateResult.Errors!);
            }
            var orderResponse = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            var addressResponse = new OrderAddressResponse(
                orderResponse.Address.Number,
                orderResponse.Address.Street,
                orderResponse.Address.Neighbourhood,
                orderResponse.Address.City,
                orderResponse.Address.Country,
                orderResponse.Address.Complement,
                orderResponse.Address.CEP
            );

            var response = new OrderResponse(
                orderResponse.Id,
                orderResponse.UserId,
                orderResponse.OrderType,
                addressResponse,
                orderResponse.PaymentInformation is null
                    ? null
                    : new PaymentInformationOrderResponse(
                        orderResponse.PaymentInformation.PaymentMethod,
                        orderResponse.PaymentInformation.PaymentName,
                        orderResponse.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or  PaymentMethod.CreditCard) 
                            ? orderResponse.PaymentInformation.PaymentKey 
                            : null,
                        orderResponse.PaymentInformation.ExpirationMonth,
                        orderResponse.PaymentInformation.ExpirationYear,
                        orderResponse.PaymentInformation.CardFlag,
                        orderResponse.PaymentInformation.Last4Digits
                    ),
                orderResponse.TotalPrice,
                orderResponse.OrderDate,
                orderResponse.Confirmation,
                orderResponse.Status,
                orderResponse.DiscountId
            );

            return Result<OrderResponse>.Success(response);
        }
    }
}
