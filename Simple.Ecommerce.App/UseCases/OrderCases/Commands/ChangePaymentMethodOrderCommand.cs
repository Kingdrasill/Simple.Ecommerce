using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Contracts.CardInformationContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ChangePaymentMethodOrderCommand : IChangePaymentMethodOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ICardService _cardService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangePaymentMethodOrderCommand(
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

        public async Task<Result<OrderPaymentMethodResponse>> Execute(OrderPaymentMethodRequest request)
        {
            var getOrder = await _repository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderPaymentMethodResponse>.Failure(getOrder.Errors!);
            }

            Result<CardInformation>? cardInformation = null;

            if (request.PaymentMethod is PaymentMethod.Cash or PaymentMethod.Pix && request.CardInformation is not null)
            {
                return Result<OrderPaymentMethodResponse>.Failure(new List<Error> { new("AddPaymentMethodOrderCommand.InvalidPaymentMethod", "Método de pagamento dinheiro ou pix não precisam das informações de cartão.") });
            }
            if (request.PaymentMethod is PaymentMethod.CreditCard or PaymentMethod.DebitCard)
            {
                if (request.CardInformation is null)
                {
                    return Result<OrderPaymentMethodResponse>.Failure(new List<Error> { new("AddPaymentMethodOrderCommand.MissingCardInformation", "Método de pagamento cartão de crédito ou débito precisam das informações de cartão.") });
                }

                var isValid = _cardService.IsValidCardNumber(request.CardInformation.CardNumber);
                if (isValid.IsFailure)
                {
                    return Result<OrderPaymentMethodResponse>.Failure(isValid.Errors!);
                }

                var cardFlag = _cardService.GetCardFlag(request.CardInformation.CardNumber);
                if (cardFlag.IsFailure)
                {
                    return Result<OrderPaymentMethodResponse>.Failure(cardFlag.Errors!);
                }

                var encryptedNumber = _cryptographyService.Encrypt(request.CardInformation.CardNumber);
                if (encryptedNumber.IsFailure)
                {
                    return Result<OrderPaymentMethodResponse>.Failure(encryptedNumber.Errors!);
                }

                var last4Digits = request.CardInformation.CardNumber[^4..];
                cardInformation = new CardInformation().Create(
                    request.CardInformation.CardHolderName,
                    encryptedNumber.GetValue(),
                    request.CardInformation.ExpirationMonth,
                    request.CardInformation.ExpirationYear,
                    cardFlag.GetValue(),
                    last4Digits
                );
                if (cardInformation.IsFailure)
                {
                    return Result<OrderPaymentMethodResponse>.Failure(cardInformation.Errors!);
                }
            }

            var instace = getOrder.GetValue();
            instace.UpdatePaymentMethod(
                request.PaymentMethod,
                cardInformation is null ? null : cardInformation.GetValue()
            );

            var updateResult = await _repository.Update(instace);
            if (updateResult.IsFailure)
            {
                return Result<OrderPaymentMethodResponse>.Failure(updateResult.Errors!);
            }
            var order = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            var response = new OrderPaymentMethodResponse
            (
                order.Id,
                order.PaymentMethod,
                order.CardInformation is null ? null : new OrderCardInformationResponse(
                    order.CardInformation.CardHolderName,
                    order.CardInformation.ExpirationMonth,
                    order.CardInformation.ExpirationYear,
                    order.CardInformation.CardFlag,
                    order.CardInformation.Last4Digits
                )
            );

            return Result<OrderPaymentMethodResponse>.Success(response);
        }
    }
}
