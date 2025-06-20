using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Queries
{
    public class GetCategoryQuery : IGetCategoryQuery
    {
        private readonly ICategoryRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCategoryQuery(
            ICategoryRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
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

            var repoResponse = await GetFromRepository(id, NoTracking);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Category>();

            return repoResponse;
        }

        private async Task<Result<CategoryResponse>> GetFromRepository(int id, bool NoTracking)
        {
            var getResult = await _repository.Get(id, NoTracking);

            if (getResult.IsFailure)
            {
                return Result<CategoryResponse>.Failure(getResult.Errors!);
            }

            var category = getResult.GetValue();

            var response = new CategoryResponse(
                category.Id,
                category.Name
            );

            return Result<CategoryResponse>.Success(response);
        }
    }
}
