using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Queries
{
    public class GetProductQuery : IGetProductQuery
    {
        private readonly IProductRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetProductQuery(
            IProductRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductResponse>> Execute(int id, bool NoTracking = true)
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

            var repoResponse = await GetFromRepository(id, NoTracking);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Product>();

            return repoResponse;
        }

        private async Task<Result<ProductResponse>> GetFromRepository(int id, bool NoTracking)
        {
            var getResult = await _repository.Get(id);
            if (getResult.IsFailure)
            {
                return Result<ProductResponse>.Failure(getResult.Errors!);
            }

            var product = getResult.GetValue();

            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.Stock
            );

            return Result<ProductResponse>.Success(response);
        }
    }
}
