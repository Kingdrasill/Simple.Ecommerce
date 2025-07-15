using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class CreateTierDiscountCommand : ICreateTierDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateTierDiscountCommand(
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
            if (getDiscountTier.IsSuccess)
            {
                return Result<DiscountTierResponse>.Failure(new List<Error> { new("CreateDiscountTierDiscountCommand.AlreadyExists", "O tier de desconto já existe!") });
            }

            var getDiscount = await _repository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(getDiscount.Errors!);
            }

            var instance = new DiscountTier().Create(
                request.Id,
                request.Name,
                request.MinQuality,
                request.Value,
                request.DiscountId
            );
            if (instance.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(instance.Errors!);
            }

            var createResult = await _discountTierRepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(createResult.Errors!);
            }
            var discountTier = createResult.GetValue();
            
            if (_useCache.Use)
                _cacheHandler.SetItemStale<DiscountTier>();

            var response = new DiscountTierResponse(
                discountTier.Id,
                discountTier.Name,
                discountTier.MinQuantity,
                discountTier.Value,
                discountTier.DiscountId
            );

            return Result<DiscountTierResponse>.Success(response);
        }
    }
}
