using Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ReviewCases.Commands
{
    public class DeleteReviewCommand : IDeleteReviewCommand
    {
        private readonly IReviewRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteReviewCommand(
            IReviewRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<Review>();

            return deleteResult;
        }
    }
}
