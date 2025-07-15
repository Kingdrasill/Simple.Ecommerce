using Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Commands
{
    public class DeleteCategoryCommand : IDeleteCategoryCommand
    {
        private readonly ICategoryRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteCategoryCommand(
            ICategoryRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _cacheHandler = cacheHandler;
            _useCache = useCache;
        }

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<Category>();

            return deleteResult;
        }
    }
}
