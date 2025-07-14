using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class DeleteUserCommand : IDeleteUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteUserCommand(
            IUserRepository repository,
            ISaverTransectioner unityOfWork,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<User>();
            }

            return deleteResult;
        }
    }
}
