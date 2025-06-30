using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UpdateDiscountBundleItemDiscountCommand : IUpdateDiscountBundleItemDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountBundleItemRepository _discountBundleItemRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateDiscountBundleItemDiscountCommand(
            IDiscountRepository repository, 
            IDiscountBundleItemRepository discountBundleItemRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountBundleItemRepository = discountBundleItemRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountBundleItemResponse>> Execute(DiscountBundleItemRequest request)
        {
            var getDiscountBundleItem = await _discountBundleItemRepository.Get(request.Id);
            if (getDiscountBundleItem.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(getDiscountBundleItem.Errors!);
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

            var updateResult = await _discountBundleItemRepository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<DiscountBundleItemResponse>.Failure(updateResult.Errors!);
            }

            var discountBundleItem = updateResult.GetValue();

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
