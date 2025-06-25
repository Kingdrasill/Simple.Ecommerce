using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Contracts.ProductCategoryContracts;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetCategoriesProductQuery : IGetCategoriesProductQuery
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetCategoriesProductQuery(
            IProductRepository repository, 
            ICategoryRepository categoryRepository,
            IProductCategoryRepository productCategoryRepository, 
            IRepositoryHandler repositoryHandler,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _repositoryHandler = repositoryHandler;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductCategoriesDTO>> Execute(int productId)
        {
            var productResponse = await GetAsync<Product>(
                () => _cacheHandler.GetFromCache<Product, Product>(productId, cache =>
                    new Product().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToDecimal(cache["Price"]),
                        Convert.ToString(cache["Description"])!,
                        Convert.ToInt32(cache["Stock"])
                    ).GetValue()),
                () => _repositoryHandler.GetFromRepository<Product>(
                    productId,
                    async (id) => await _repository.Get(id)),
                () => _cacheHandler.SendToCache<Product>()
            );
            if (productResponse.IsFailure)
            {
                return Result<ProductCategoriesDTO>.Failure(productResponse.Errors!);
            }

            var productCategoryResponse = await GetAsync<List<ProductCategory>>(
                () => _cacheHandler.ListFromCacheByProperty<ProductCategory, ProductCategory>(nameof(ProductCategory.ProductId), productId, cache =>
                    new ProductCategory().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToInt32(cache["CategoryId"])
                    ).GetValue()),
                () => _repositoryHandler.ListFromRepository<ProductCategory>(
                    productId,
                    async (filterId) => await _productCategoryRepository.GetByProductId(filterId)),
                () => _cacheHandler.SendToCache<ProductCategory>()
            );
            if (productCategoryResponse.IsFailure)
            {
                return Result<ProductCategoriesDTO>.Failure(productCategoryResponse.Errors!);
            }

            List<int> categoriesIds = productCategoryResponse.GetValue()
                .Select(pc => pc.CategoryId)
                .ToList();

            var categoryResponse = await GetAsync<List<Category>>(
                () => _cacheHandler.ListFromCacheByPropertyIn<Category, Category>(nameof(Category.Id), categoriesIds.Cast<object>(), cache =>
                    new Category().Create(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!
                    ).GetValue()),
                () => _repositoryHandler.GetFromRepositoryByIds<Category, int>(
                    categoriesIds,
                    async (ids) => await _categoryRepository.GetCategoriesByIds(ids.ToList())),
                () => _cacheHandler.SendToCache<Category>()
            );
            if (categoryResponse.IsFailure)
            {
                return Result<ProductCategoriesDTO>.Failure(categoryResponse.Errors!);
            }

            List<CategoryResponse> categoriesResponses = new();
            foreach (var category in categoryResponse.GetValue()) 
            {
                categoriesResponses.Add(new CategoryResponse(
                    category.Id,
                    category.Name
                ));
            }

            var reponse = new ProductCategoriesDTO(
                productId,
                productResponse.GetValue().Name,
                categoriesResponses
            );

            return Result<ProductCategoriesDTO>.Success(reponse);
        }

        private async Task<Result<TResponse>> GetAsync<TResponse>(
            Func<Result<TResponse>> getFromCache,
            Func<Task<Result<TResponse>>> getFromRepo,
            Func<Task> sendToCache
        )
        {
            if (_useCache.Use)
            {
                var cacheResponse = getFromCache();
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await getFromRepo();
            if (repoResponse.IsSuccess && _useCache.Use)
                await sendToCache();

            return repoResponse;
        }
    }
}
