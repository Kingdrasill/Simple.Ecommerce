using Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ReviewCases.Commands
{
    public class UpdateReviewCommand :IUpdateReviewCommand
    {
        private readonly IReviewRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateReviewCommand(
            IReviewRepository repository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            ISaverTransectioner unityOfWork,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ReviewResponse>> Execute(ReviewRequest request)
        {
            var getReview = await _repository.Get(request.Id);
            if (getReview.IsFailure)
            {
                return Result<ReviewResponse>.Failure(getReview.Errors!);
            }

            var getUser = await _userRepository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<ReviewResponse>.Failure(getUser.Errors!);
            }

            var getProduct = await _productRepository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<ReviewResponse>.Failure(getProduct.Errors!);
            }

            var instance = new Review().Create(
                request.Id,
                request.ProductId,
                request.UserId,
                request.Score,
                request.Comment
            );
            if (instance.IsFailure)
            {
                return Result<ReviewResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<ReviewResponse>.Failure(updateResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<ReviewResponse>.Failure(commit.Errors!);
            }

            var review = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Review>();

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
