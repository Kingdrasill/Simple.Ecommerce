﻿using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.UseCases.ReviewCases.Queries
{
    public class GetReviewQuery : IGetReviewQuery
    {
        private readonly IReviewRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public GetReviewQuery(
            IReviewRepository repository,
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

        public async Task<Result<ReviewResponse>> Execute(int id)
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

            var repoResponse = await _repositoryHandler.GetFromRepository<Review, ReviewResponse>(
                id,
                async (id) => await _repository.Get(id),
                review => new ReviewResponse(
                    review.Id,
                    review.Score,
                    review.Comment,
                    review.UserId,
                    review.ProductId
                ));
            if (repoResponse.IsSuccess && _useCache.Use)
                await _cacheHandler.SendToCache<Review>();

            return repoResponse;
        }
    }
}
