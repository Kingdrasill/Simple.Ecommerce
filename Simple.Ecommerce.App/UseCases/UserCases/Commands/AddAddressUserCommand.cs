using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class AddAddressUserCommand : IAddAddressUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IUserAddressRepository _userAddressrepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddAddressUserCommand(
            IUserRepository repository, 
            IUserAddressRepository userAddressrepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userAddressrepository = userAddressrepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(UserAddressRequest request)
        {
            var getUser = await _repository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<bool>.Failure(getUser.Errors!);
            }

            var address = new Address(
                request.Address.Number,
                request.Address.Street,
                request.Address.Neighbourhood,
                request.Address.City,
                request.Address.Country,
                request.Address.Complement,
                request.Address.CEP
            );
            var instance = new UserAddress().Create(
                0,
                request.UserId,
                address
            );
            if (instance.IsFailure)
            {
                return Result<bool>.Failure(instance.Errors!);
            }

            var createResult = await _userAddressrepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<bool>.Failure(createResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<UserAddress>();

            return Result<bool>.Success(true);
        }
    }
}
