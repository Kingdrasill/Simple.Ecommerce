using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemoveAddressUserCommand : IRemoveAddressUserCommand
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveAddressUserCommand(
            IUserAddressRepository userAddressRepository,
            IUserRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _userAddressRepository = userAddressRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int userAddressId)
        {
            var deleteResult = await _userAddressRepository.Delete(userAddressId);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<UserAddress>();

            return deleteResult;
        }
    }
}
