using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.CardInformationContracts;
using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetCardsUserQuery : IGetCardsUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly IUserCardRepository _userCardRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCardsUserQuery(
            IUserRepository repository, 
            IUserCardRepository userCardRepository,
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userCardRepository = userCardRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserCardsReponse>> Execute(int userId)
        {
            var userResponse = await GetAsync<UserResponse>(
                () => _cacheHandler.GetFromCache<User, UserResponse>(userId, cache =>
                    new UserResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToString(cache["Email"])!,
                        Convert.ToString(cache["PhoneNumber"])!
                    )),
                () => _repositoryHandler.GetFromRepository<User, UserResponse>(
                    userId,
                    async (id) => await _repository.Get(id),
                    user => new UserResponse(
                        user.Id,
                        user.Name,
                        user.Email,
                        user.PhoneNumber
                    )),
                () => _cacheHandler.SendToCache<User>()
            );
            if (userResponse.IsFailure)
            {
                return Result<UserCardsReponse>.Failure(userResponse.Errors!);
            }

            var cardsResponse = await GetAsync<List<UserCardInformationResponse>>(
                () => _cacheHandler.ListFromCacheByProperty<UserCard, UserCardInformationResponse>(nameof(UserCard.UserId), userId, cache => 
                    new UserCardInformationResponse(
                        Convert.ToString(cache["CardHolderName"])!,
                        Convert.ToString(cache["ExpirationMonth"])!,
                        Convert.ToString(cache["ExpirationYear"])!,
                        (CardFlag)Convert.ToInt32(cache["CardFlag"]),
                        Convert.ToString(cache["Last4Digits"])!,
                        Convert.ToInt32(cache["Id"])
                    )),
                () => _repositoryHandler.ListFromRepository<UserCard, UserCardInformationResponse>(
                    userId,
                    async (filterId) => await _userCardRepository.GetByUserId(filterId),
                    cardInformation => new UserCardInformationResponse(
                        cardInformation.CardInformation.CardHolderName,
                        cardInformation.CardInformation.ExpirationMonth,
                        cardInformation.CardInformation.ExpirationYear,
                        cardInformation.CardInformation.CardFlag,
                        cardInformation.CardInformation.Last4Digits,
                        cardInformation.Id
                    )),
                () => _cacheHandler.SendToCache<UserCard>()
            );
            if (cardsResponse.IsFailure)
            {
                return Result<UserCardsReponse>.Failure(cardsResponse.Errors!);
            }

            var response = new UserCardsReponse(
                userId,
                userResponse.GetValue().Name,
                cardsResponse.GetValue()
            );

            return Result<UserCardsReponse>.Success(response);
        }

        private async Task<Result<TResponse>> GetAsync<TResponse>(
            Func<Result<TResponse>> getFromCache,
            Func<Task<Result<TResponse>>> getFromRepo,
            Func<Task> sendToCache
        )
        {
            if (_useCache.Use)
            {
                var cacheResponse = getFromCache();
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await getFromRepo();
            if (repoResponse.IsSuccess && _useCache.Use)
                await sendToCache();

            return repoResponse;
        }
    }
}
