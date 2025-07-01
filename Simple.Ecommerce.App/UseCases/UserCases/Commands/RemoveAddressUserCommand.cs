using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemoveAddressUserCommand : IRemoveAddressUserCommand
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveAddressUserCommand(
            IUserAddressRepository userAddressRepository,
            ISaverTransectioner unityOfWork,
            IUserRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _userAddressRepository = userAddressRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int userAddressId)
        {
            var deleteResult = await _userAddressRepository.Delete(userAddressId);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<UserAddress>();
            }

            return deleteResult;
        }
    }
}
