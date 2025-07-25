using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ChangePaymentInformationOrderCommand : IChangePaymentInformationOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ICardService _cardService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangePaymentInformationOrderCommand(
            IOrderRepository repository, 
            ICryptographyService cryptographyService,
            ICardService cardService,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _cryptographyService = cryptographyService;
            _cardService = cardService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderPaymentInformationResponse>> Execute(OrderPaymentInformationRequest request)
        {
            var getOrder = await _repository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderPaymentInformationResponse>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not (OrderLock.Unlock or OrderLock.LockPrice))
            {
                return Result<OrderPaymentInformationResponse>.Failure(new List<Error> { new("OrderPaymentInformationResponse.OrderLocked", "O pedido está bloqueado à mudança de dados do pedido.") });
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
                        encryptedKey = request.PaymentInformation.PaymentKey;
                        break;
                    case (PaymentMethod.CreditCard or PaymentMethod.DebitCard):
                        var isValid = _cardService.IsValidCardNumber(request.PaymentInformation.PaymentKey!);
                        if (isValid.IsFailure)
                        {
                            return Result<OrderPaymentInformationResponse>.Failure(isValid.Errors!);
                        }

                        var cardFlagResult = _cardService.GetCardFlag(request.PaymentInformation.PaymentKey!);
                        if (cardFlagResult.IsFailure)
                        {
                            return Result<OrderPaymentInformationResponse>.Failure(cardFlagResult.Errors!);
                        }
                        cardFlag = cardFlagResult.GetValue();

                        var encryptedKeyResult = _cryptographyService.Encrypt(request.PaymentInformation.PaymentKey!);
                        if (encryptedKeyResult.IsFailure)
                        {
                            return Result<OrderPaymentInformationResponse>.Failure(encryptedKeyResult.Errors!);
                        }
                        encryptedKey = encryptedKeyResult.GetValue();
                        last4Digits = request.PaymentInformation.PaymentKey![^4..];
                        break;
                    default:
                        return Result<OrderPaymentInformationResponse>.Failure(new List<Error> { new("ChangePaymentInformationOrderCommand.InvalidPaymentMethod", "O método de pagamento passado não é válido.") });
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
            order.UpdatePaymentInformation(paymentInformation);
            if (order.Status is not ("Pending Payment" or  "Failed Payment"))
                order.UpdateStatus("Altered", order.OrderLock);
            if (order.Validate() is { IsFailure: true } result)
            {
                return Result<OrderPaymentInformationResponse>.Failure(result.Errors!);
            }

            var updateResult = await _repository.Update(order);
            if (updateResult.IsFailure)
            {
                return Result<OrderPaymentInformationResponse>.Failure(updateResult.Errors!);
            }
            var orderResponse = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            var response = new OrderPaymentInformationResponse
            (
                orderResponse.Id,
                orderResponse.PaymentInformation is null ? null : new PaymentInformationOrderResponse(
                    orderResponse.PaymentInformation.PaymentMethod,
                    orderResponse.PaymentInformation.PaymentName,
                    orderResponse.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or PaymentMethod.CreditCard)
                        ? orderResponse.PaymentInformation.PaymentKey
                        : null,
                    orderResponse.PaymentInformation.ExpirationMonth,
                    orderResponse.PaymentInformation.ExpirationYear,
                    orderResponse.PaymentInformation.CardFlag,
                    orderResponse.PaymentInformation.Last4Digits
                )
            );

            return Result<OrderPaymentInformationResponse>.Success(response);
        }
    }
}
