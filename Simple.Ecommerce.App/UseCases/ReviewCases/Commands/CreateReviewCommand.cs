using Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ReviewCases.Commands
{
    public class CreateReviewCommand : ICreateReviewCommand
    {
        private readonly IReviewRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateReviewCommand(
            IReviewRepository repository,
            IUserRepository userRepository, 
            IProductRepository productRepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ReviewResponse>> Execute(ReviewRequest request)
        {
            var getReview = await _repository.Get(request.Id);
            if (getReview.IsSuccess)
            {
                return Result<ReviewResponse>.Failure(new List<Error> { new("CreateReviewCommand.AlreadyExists", "A avialiação já foi criada!") });
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

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<ReviewResponse>.Failure(createResult.Errors!);
            }
            var review = createResult.GetValue();

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
