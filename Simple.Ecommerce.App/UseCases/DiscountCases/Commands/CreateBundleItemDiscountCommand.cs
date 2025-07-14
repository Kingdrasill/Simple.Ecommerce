using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class CreateBundleItemDiscountCommand : ICreateBundleItemDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateBundleItemDiscountCommand(
            IDiscountRepository repository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountBundleItemResponse>> Execute(DiscountBundleItemRequest request)
        {
            var getDiscountBundleItem = await _discountBundleItemRepository.Get(request.Id);
            if (getDiscountBundleItem.IsSuccess)
            {
                return Result<DiscountBundleItemResponse>.Failure(new List<Error> { new("CreateDiscountBundleItemDiscountCommand.AlreadyExists", "O item do pacote do desconto já exise!") });
            }

            var getDiscount = await _repository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(getDiscount.Errors!);
            }

            var instance = new DiscountBundleItem().Create(
                request.Id,
                request.DiscountId,
                request.ProductId,
                request.Quantity
            );
            if (instance.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(instance.Errors!);
            }

            var createResult = await _discountBundleItemRepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(createResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(commit.Errors!);
            }

            var discountBundleItem = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<DiscountBundleItem>();

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
