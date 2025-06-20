using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Queries
{
    public class ListDiscountBundleItemDiscountQuery : IListDiscountBundleItemDiscountQuery
    {
        private readonly IDiscountBundleItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListDiscountBundleItemDiscountQuery(
            IDiscountBundleItemRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<DiscountBundleItemResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<DiscountBundleItem, DiscountBundleItemResponse>(cache =>
                    new DiscountBundleItemResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["ProductId"]),
                        Convert.ToInt32(cache["Quantity"]),
                        Convert.ToInt32(cache["DiscountId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<DiscountBundleItem>();

            return repoResponse;
        }

        private async Task<Result<List<DiscountBundleItemResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<DiscountBundleItemResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<DiscountBundleItemResponse>();
            foreach (var discountBundleItem in listResult.GetValue())
            {
                response.Add(new DiscountBundleItemResponse(
                    discountBundleItem.Id,
                    discountBundleItem.ProductId,
                    discountBundleItem.Quantity,
                    discountBundleItem.DiscountId
                ));
            }

            return Result<List<DiscountBundleItemResponse>>.Success(response);
        }
    }
}
