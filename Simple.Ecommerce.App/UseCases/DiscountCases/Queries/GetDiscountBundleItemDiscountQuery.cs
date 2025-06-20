using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class GetDiscountBundleItemDiscountQuery : IGetDiscountBundleItemDiscountQuery
    {
        private readonly IDiscountBundleItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetDiscountBundleItemDiscountQuery(
            IDiscountBundleItemRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountBundleItemResponse>> Execute(int id)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<DiscountBundleItem, DiscountBundleItemResponse>(id, cache =>
                    new DiscountBundleItemResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToInt32(cache["Quantity"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository(id);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountBundleItem>();

            return repoResponse;
        }

        private async Task<Result<DiscountBundleItemResponse>> GetFromRepository(int id) 
        {
            var getResult = await _repository.Get(id);
            if (getResult.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(getResult.Errors!);
            }

            var discountBundleItem = getResult.GetValue();
            var response = new DiscountBundleItemResponse(
                discountBundleItem.Id,
                discountBundleItem.ProductId,
                discountBundleItem.Quantity,
                discountBundleItem.DiscountId
            );

            return Result<DiscountBundleItemResponse>.Success(response);
        }
    }
}
