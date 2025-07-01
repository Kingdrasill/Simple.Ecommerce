using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetAddressesUserQuery : IGetAddressesUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetAddressesUserQuery(
            IUserRepository repository, 
            IUserAddressRepository userAddressRepository,
            IRepositoryHandler repositoryHandler,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userAddressRepository = userAddressRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<UserAddressesResponse>> Execute(int userId)
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
                return Result<UserAddressesResponse>.Failure(userResponse.Errors!);
            }

            var addressesResponse = await GetAsync<List<UserAddressResponse>>(
                () => _cacheHandler.ListFromCacheByProperty<UserAddress, UserAddressResponse>(nameof(UserAddress.UserId), userId,
                    cache => new UserAddressResponse(
                        Convert.ToInt32(cache[$"{nameof(Address)}_Number"]),
                        Convert.ToString(cache[$"{nameof(Address)}_Street"])!,
                        Convert.ToString(cache[$"{nameof(Address)}_Neighbourhood"])!,
                        Convert.ToString(cache[$"{nameof(Address)}_City"])!,
                        Convert.ToString(cache[$"{nameof(Address)}_Country"])!,
                        cache.GetNullableString($"{nameof(Address)}_Complement"),
                        Convert.ToString(cache[$"{nameof(Address)}_CEP"])!,
                        Convert.ToInt32(cache["Id"])
                    )),
                () => _repositoryHandler.ListFromRepository<UserAddress, UserAddressResponse>(
                    userId,
                    async (filterId) => await _userAddressRepository.GetByUserId(filterId),
                    userAddress => new UserAddressResponse(
                        userAddress.Address.Number,
                        userAddress.Address.Street,
                        userAddress.Address.Neighbourhood,
                        userAddress.Address.City,
                        userAddress.Address.Country,
                        userAddress.Address.Complement,
                        userAddress.Address.CEP,
                        userAddress.Id
                    )),
                () => _cacheHandler.SendToCache<UserAddress>()
            );
            if (addressesResponse.IsFailure)
            {
                return Result<UserAddressesResponse>.Failure(addressesResponse.Errors!);
            }

            var result = new UserAddressesResponse(
                userId,
                userResponse.GetValue().Name,
                addressesResponse.GetValue()
            );

            return Result<UserAddressesResponse>.Success(result);
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
