using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Contracts.UserPaymentContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetPaymentsUserQuery : IGetPaymentsUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly IUserPaymentRepository _userPaymentRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetPaymentsUserQuery(
            IUserRepository repository, 
            IUserPaymentRepository userPaymentRepository,
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userPaymentRepository = userPaymentRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserPaymentsResponse>> Execute(int userId)
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
                return Result<UserPaymentsResponse>.Failure(userResponse.Errors!);
            }

            var paymentsResponse = await GetAsync<List<PaymentInformationUserResponse>>(
                () => _cacheHandler.ListFromCacheByProperty<UserPayment, PaymentInformationUserResponse>(nameof(UserPayment.UserId), userId, 
                    cache => new PaymentInformationUserResponse(
                        Convert.ToInt32(cache["Id"]),
                        (PaymentMethod)Convert.ToInt32(cache[$"{nameof(PaymentInformation)}_PaymentMethod"]),
                        cache.GetNullableString($"{nameof(PaymentInformation)}_PaymentName"),
                        (PaymentMethod)Convert.ToInt32(cache[$"{nameof(PaymentInformation)}_PaymentMethod"]) is not (PaymentMethod.CreditCard or PaymentMethod.DebitCard)
                            ? cache.GetNullableString($"{nameof(PaymentInformation)}_PaymentKey")
                            : null,
                        cache.GetNullableString($"{nameof(PaymentInformation)}_ExpirationMonth"),
                        cache.GetNullableString($"{nameof(PaymentInformation)}_ExpirationYear"),
                        cache.GetNullableCardFlag($"{nameof(PaymentInformation)}_CardFlag"),
                        cache.GetNullableString("Last4Digits")
                    )),
                () => _repositoryHandler.ListFromRepository<UserPayment, PaymentInformationUserResponse>(
                    userId,
                    async (filterId) => await _userPaymentRepository.GetByUserId(filterId),
                    paymentInformation => new PaymentInformationUserResponse(
                        paymentInformation.Id,
                        paymentInformation.PaymentInformation.PaymentMethod,
                        paymentInformation.PaymentInformation.PaymentName,
                        paymentInformation.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or PaymentMethod.CreditCard)
                            ? paymentInformation.PaymentInformation.PaymentKey
                            : null,
                        paymentInformation.PaymentInformation.ExpirationMonth,
                        paymentInformation.PaymentInformation.ExpirationYear,
                        paymentInformation.PaymentInformation.CardFlag,
                        paymentInformation.PaymentInformation.Last4Digits
                    )),
                () => _cacheHandler.SendToCache<UserPayment>()
            );
            if (paymentsResponse.IsFailure)
            {
                return Result<UserPaymentsResponse>.Failure(paymentsResponse.Errors!);
            }

            var response = new UserPaymentsResponse(
                userId,
                paymentsResponse.GetValue()
            );

            return Result<UserPaymentsResponse>.Success(response);
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
