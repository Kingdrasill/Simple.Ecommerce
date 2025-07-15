using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class DeleteCouponDiscountCommand : IDeleteCouponDiscountCommand
    {
        private readonly ICouponRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteCouponDiscountCommand(
            ICouponRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);
            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<Coupon>();
            
            return deleteResult;
        }
    }
}
