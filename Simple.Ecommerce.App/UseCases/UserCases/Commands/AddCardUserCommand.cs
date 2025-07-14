using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddCardUserCommand : IAddCardUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IUserCardRepository _userCardRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly ICryptographyService _cryptographyService;
        private readonly ICardService _cardService;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddCardUserCommand(
            IUserRepository repository, 
            IUserCardRepository userCardRepository,
            ISaverTransectioner unityOfWork,
            ICryptographyService cryptographyService,
            ICardService cardService,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userCardRepository = userCardRepository;
            _saverOrTransectioner = unityOfWork;
            _cryptographyService = cryptographyService;
            _cardService = cardService;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(UserCardRequest request)
        {
            var getUser = await _repository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<bool>.Failure(getUser.Errors!);
            }

            var isValid = _cardService.IsValidCardNumber(request.CardInformation.CardNumber);
            if (isValid.IsFailure)
            {
                return Result<bool>.Failure(isValid.Errors!);
            }

            var cardFlag = _cardService.GetCardFlag(request.CardInformation.CardNumber);
            if (cardFlag.IsFailure)
            {
                return Result<bool>.Failure(cardFlag.Errors!);
            }

            var encryptedNumber = _cryptographyService.Encrypt(request.CardInformation.CardNumber);
            if (encryptedNumber.IsFailure)
            {
                return Result<bool>.Failure(encryptedNumber.Errors!);
            }
            var last4Digits = request.CardInformation.CardNumber[^4..];

            var cardInformation = new CardInformation().Create(
                request.CardInformation.CardHolderName,
                encryptedNumber.GetValue(),
                request.CardInformation.ExpirationMonth,
                request.CardInformation.ExpirationYear,
                cardFlag.GetValue(),
                last4Digits
            );
            if (cardInformation.IsFailure)
            {
                return Result<bool>.Failure(cardInformation.Errors!);
            }

            var instance = new UserCard().Create(
                0,
                request.UserId,
                cardInformation.GetValue()
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

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<bool>.Failure(commit.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<UserCard>();

            return Result<bool>.Success(true);
        }
    }
}
