using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetProductQuery : IGetProductQuery
    {
        private readonly IProductRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetProductQuery(
            IProductRepository repository,
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

        public async Task<Result<ProductResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Product, ProductResponse>(id, cache => 
                    new ProductResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToString(cache["Name"])!,
                        Convert.ToDecimal(cache["Price"]),
                        Convert.ToString(cache["Description"])!,
                        Convert.ToInt32(cache["Stock"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await _repositoryHandler.GetFromRepository<Product, ProductResponse>(
                id,
                async (id) => await _repository.Get(id),
                product => new ProductResponse(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Description,
                    product.Stock
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Product>();

            return repoResponse;
        }
    }
}
