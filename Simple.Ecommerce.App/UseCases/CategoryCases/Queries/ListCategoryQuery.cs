using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Queries
{
    public class ListCategoryQuery : IListCategoryQuery
    {
        private readonly ICategoryRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListCategoryQuery(
            ICategoryRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<CategoryResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Category, CategoryResponse>(cache => 
                    new CategoryResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Category>();

            return repoResponse;
        }

        private async Task<Result<List<CategoryResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<CategoryResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<CategoryResponse>();
            foreach (var category in listResult.GetValue())
            {
                response.Add(new CategoryResponse(
                    category.Id,
                    category.Name
                ));
            }

            return Result<List<CategoryResponse>>.Success(response);
        }
    }
}
