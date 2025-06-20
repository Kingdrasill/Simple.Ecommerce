using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UpdateDiscountTierDiscountCommand : IUpdateDiscountTierDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateDiscountTierDiscountCommand(
            IDiscountRepository repository, 
            IDiscountTierRepository discountTierRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountTierRepository = discountTierRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountTierResponse>> Execute(DiscountTierRequest request)
        {
            var getDiscountTier = await _discountTierRepository.Get(request.Id);
            if (getDiscountTier.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(getDiscountTier.Errors!);
            }

            var getDiscount = await _repository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(getDiscount.Errors!);
            }

            var instance = new DiscountTier().Create(
                request.Id,
                request.MinQuality,
                request.Value,
                request.DiscountId
            );
            if (instance.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _discountTierRepository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(updateResult.Errors!);
            }

            var discountTier = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<DiscountTier>();

            var response = new DiscountTierResponse(
                discountTier.Id,
                discountTier.MinQuantity,
                discountTier.Value,
                discountTier.DiscountId
            );

            return Result<DiscountTierResponse>.Success(response);
        }
    }
}
