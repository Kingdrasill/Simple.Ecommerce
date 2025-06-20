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
    public class GetReviewQuery : IGetReviewQuery
    {
        private readonly IReviewRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetReviewQuery(
            IReviewRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ReviewResponse>> Execute(int id, bool NoTracking = true)
        {
            if (_useCache.Use)
            {
                var cacheResponse = _cacheHandler.GetFromCache<Review, ReviewResponse>(id, cache =>
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

            var repoResponse = await GetFromRepository(id, NoTracking);
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Review>();

            return repoResponse;
        }

        private async Task<Result<ReviewResponse>> GetFromRepository(int id, bool NoTracking)
        {
            var getResult = await _repository.Get(id, NoTracking);
            if (getResult.IsFailure)
            {
                return Result<ReviewResponse>.Failure(getResult.Errors!);
            }

            var review = getResult.GetValue();
            var response = new ReviewResponse(
                review.Id,
                review.Score,
                review.Comment,
                review.UserId,
                review.ProductId
            );

            return Result<ReviewResponse>.Success(response);
        }
    }
}
