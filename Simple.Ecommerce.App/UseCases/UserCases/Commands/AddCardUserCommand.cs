using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddCardUserCommand : IAddCardUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IUserCardRepository _userCardRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddCardUserCommand(
            IUserRepository repository, 
            IUserCardRepository userCardRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userCardRepository = userCardRepository;
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

            var last4Digits = request.CardInformation.CardNumber[^4..];

            // Add Crypthography for card number
            // Add Verification for card number
            // Add GetFlag for card number

            var cardInformation = new CardInformation().Create(
                request.CardInformation.CardHolderName,
                request.CardInformation.CardNumber,
                request.CardInformation.ExpirationMonth,
                request.CardInformation.ExpirationYear,
                CardFlag.AmericanExpress, // This should be replaced with a method to get the actual card flag
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

            if (_useCache.Use)
                _cacheHandler.SetItemStale<UserCard>();

            return Result<bool>.Success(true);
        }
    }
}
