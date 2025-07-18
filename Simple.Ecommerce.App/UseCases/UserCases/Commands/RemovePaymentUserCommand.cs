using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.UserCases.Commands
{
    public class RemovePaymentUserCommand : IRemovePaymentUserCommand
    {
        private readonly IUserPaymentRepository _userPaymentRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePaymentUserCommand(
            IUserPaymentRepository userPaymentRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _userPaymentRepository = userPaymentRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int userCardId)
        {
            var deleteResult = await _userPaymentRepository.Delete(userCardId);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<UserPayment>();

            return deleteResult;
        }
    }
}
