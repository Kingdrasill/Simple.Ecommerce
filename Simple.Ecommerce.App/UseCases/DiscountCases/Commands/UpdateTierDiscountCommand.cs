using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UpdateTierDiscountCommand : IUpdateTierDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly IDiscountTierRepository _discountTierRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateTierDiscountCommand(
            IDiscountRepository repository, 
            IDiscountTierRepository discountTierRepository, 
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountTierRepository = discountTierRepository;
            _saverOrTransectioner = unityOfWork;
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
                request.Name,
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

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<DiscountTierResponse>.Failure(commit.Errors!);
            }

            var discountTier = updateResult.GetValue();

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
