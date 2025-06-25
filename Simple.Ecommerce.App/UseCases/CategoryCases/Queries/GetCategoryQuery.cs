using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Queries
{
    public class GetCategoryQuery : IGetCategoryQuery
    {
        private readonly ICategoryRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCategoryQuery(
            ICategoryRepository repository,
            IRepositoryHandler repositoryHandler,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<CategoryResponse>> Execute(int id, bool NoTracking = true)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Category, CategoryResponse>(id, cache => 
                    new CategoryResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Category, CategoryResponse>(
                id,
                async (id) => await _repository.Get(id, NoTracking),
                category => new CategoryResponse(  
                    category.Id,
                    category.Name
                )
            );
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Category>();

            return repoResponse;
        }
    }
}
