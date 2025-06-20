using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Cache;

namespace Simple.Ecommerce.App.UseCases.ReviewCases.Queries
{
    public class ListReviewQuery : IListReviewQuery
    {
        private readonly IReviewRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ListReviewQuery(
            IReviewRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<ReviewResponse>>> Execute()
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.ListFromCache<Review, ReviewResponse>(cache =>
                    new ReviewResponse(
                        Convert.ToInt32(cache["Id"]),
                        Convert.ToInt32(cache["Score"]),
                        cache.GetNullableString("Comment"),
                        Convert.ToInt32(cache["UserId"]),
                        Convert.ToInt32(cache["ProductId"])
                    ));
                if (cacheResponse.IsSuccess)
                    return cacheResponse;
            }

            var repoResponse = await GetFromRepository();
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Review>();

            return repoResponse;
        }

        private async Task<Result<List<ReviewResponse>>> GetFromRepository()
        {
            var listResult = await _repository.List();
            if (listResult.IsFailure)
            {
                return Result<List<ReviewResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<ReviewResponse>();
            foreach (var review in listResult.GetValue())
            {
                response.Add(new ReviewResponse(
                    review.Id,
                    review.Score,
                    review.Comment,
                    review.UserId,
                    review.ProductId
                ));
            }

            return Result<List<ReviewResponse>>.Success(response);
        }
    }
}
