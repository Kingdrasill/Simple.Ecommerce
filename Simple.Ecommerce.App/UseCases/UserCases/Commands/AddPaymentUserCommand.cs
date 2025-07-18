using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Contracts.UserPaymentContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddPaymentUserCommand : IAddPaymentUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IUserPaymentRepository _userCardRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ICardService _cardService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddPaymentUserCommand(
            IUserRepository repository, 
            IUserPaymentRepository userCardRepository,
            ICryptographyService cryptographyService,
            ICardService cardService,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userCardRepository = userCardRepository;
            _cryptographyService = cryptographyService;
            _cardService = cardService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(UserPaymentRequest request)
        {
            var getUser = await _repository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<bool>.Failure(getUser.Errors!);
            }

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
                        return Result<bool>.Failure(isValid.Errors!);
                    }

                    var cardFlagResult = _cardService.GetCardFlag(request.PaymentInformation.PaymentKey!);
                    if (cardFlagResult.IsFailure)
                    {
                        return Result<bool>.Failure(cardFlagResult.Errors!);
                    }
                    cardFlag = cardFlagResult.GetValue();

                    var encryptedKeyResult = _cryptographyService.Encrypt(request.PaymentInformation.PaymentKey!);
                    if (encryptedKeyResult.IsFailure)
                    {
                        return Result<bool>.Failure(encryptedKeyResult.Errors!);
                    }
                    encryptedKey = encryptedKeyResult.GetValue();
                    last4Digits = request.PaymentInformation.PaymentKey![^4..];
                    break;
                default:
                    return Result<bool>.Failure(new List<Error> { new("AddPaymentUserCommand.InvalidPaymentMethod", "O método de pagamento passado não é válido.") });
            }

            var paymentInformation = new PaymentInformation().Create(
                request.PaymentInformation.PaymentMethod,
                request.PaymentInformation.PaymentName,
                encryptedKey,
                request.PaymentInformation.ExpirationMonth,
                request.PaymentInformation.ExpirationYear,
                cardFlag,
                last4Digits
            );
            if (paymentInformation.IsFailure)
            {
                return Result<bool>.Failure(paymentInformation.Errors!);
            }

            var instance = new UserPayment().Create(
                0,
                request.UserId,
                paymentInformation.GetValue()
            );
            if (instance.IsFailure)
            {
                return Result<bool>.Failure(instance.Errors!);
            }

            var createResult = await _userCardRepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<bool>.Failure(createResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<UserPayment>();

            return Result<bool>.Success(true);
        }
    }
}
