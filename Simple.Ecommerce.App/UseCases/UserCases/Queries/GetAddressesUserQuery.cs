using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Queries
{
    public class GetAddressesUserQuery : IGetAddressesUserQuery
    {
        private readonly IUserRepository _repository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetAddressesUserQuery(
            IUserRepository repository, 
            IUserAddressRepository userAddressRepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userAddressRepository = userAddressRepository;
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
                () => GetFromRepositoryUser(userId),
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
                () => GetFromRepositoryUserAddresses(userId),
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

        private async Task<Result<UserResponse>> GetFromRepositoryUser(int userId)
        {
            var getUser = await _repository.Get(userId);
            if (getUser.IsFailure)
            {
                return Result<UserResponse>.Failure(getUser.Errors!);
            }

            var user = getUser.GetValue();
            var response = new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.PhoneNumber
            );

            return Result<UserResponse>.Success(response);
        }

        private async Task<Result<List<UserAddressResponse>>> GetFromRepositoryUserAddresses(int userId)
        {
            var listResult = await _userAddressRepository.GetByUserId(userId);
            if (listResult.IsFailure)
            {
                return Result<List<UserAddressResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<UserAddressResponse>();
            foreach (var address in listResult.GetValue())
            {
                response.Add(new UserAddressResponse(
                    address.Address.Number,
                    address.Address.Street,
                    address.Address.Neighbourhood,
                    address.Address.City,
                    address.Address.Country,
                    address.Address.Complement,
                    address.Address.CEP,
                    address.Id
                ));
            }

            return Result<List<UserAddressResponse>>.Success(response);
        }
    }
}
