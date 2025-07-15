using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class DeleteBundleItemDiscountCommand : IDeleteBundleItemDiscountCommand
    {
        private readonly IDiscountBundleItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteBundleItemDiscountCommand(
            IDiscountBundleItemRepository repository, 
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
                _cacheHandler.SetItemStale<DiscountBundleItem>();

            return deleteResult;
        }
    }
}
