using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class RemoveCategoryProductCommand : IRemoveCategoryProductCommand
    {
        private readonly IProductCategoryRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveCategoryProductCommand(
            IProductCategoryRepository repository, 
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
                _cacheHandler.SetItemStale<ProductCategory>();

            return deleteResult;
        }
    }
}
