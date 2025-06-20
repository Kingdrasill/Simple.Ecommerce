using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class ListProductQuery : IListProductQuery
    {
        private readonly IProductRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListProductQuery(
            IProductRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<ProductResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Product, ProductResponse>(cache =>
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

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Product>();

            return repoResponse;
        }

        private async Task<Result<List<ProductResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();

            if (listResult.IsFailure)
            {
                return new Result<List<ProductResponse>>(
                    new List<ProductResponse>(),
                    listResult.Errors,
                    false,
                    true
                );
            }

            var response = new List<ProductResponse>();
            foreach (var product in listResult.GetValue())
            {
                response.Add(new ProductResponse(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Description,
                    product.Stock
                ));
            }

            return new Result<List<ProductResponse>>(
                response,
                null,
                true,
                false
            );
        }
    }
}
