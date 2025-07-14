using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemoveCardUserCommand : IRemoveCardUserCommand
    {
        private readonly IUserCardRepository _userCardRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveCardUserCommand(
            IUserCardRepository userCardRepository, 
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _userCardRepository = userCardRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int userCardId)
        {
            var deleteResult = await _userCardRepository.Delete(userCardId);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<UserCard>();
            }

            return deleteResult;
        }
    }
}
