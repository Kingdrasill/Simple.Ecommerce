using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UpdateDiscountCommand : IUpdateDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateDiscountCommand(
            IDiscountRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountCompleteDTO>> Execute(DiscountRequest request)
        {
            var getDiscount = await _repository.Get(request.Id);
            if (getDiscount.IsFailure)
            {
                return Result<DiscountCompleteDTO>.Failure(getDiscount.Errors!);
            }

            var instance = new Discount().Create(
                request.Id,
                request.Name,
                request.DiscountType,
                request.DiscountScope,
                request.DiscountValueType,
                request.Value,
                request.ValidFrom,
                request.ValidTo,
                request.IsActive
            );
            if (instance.IsFailure)
            {
                return Result<DiscountCompleteDTO>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<DiscountCompleteDTO>.Failure(updateResult.Errors!);
            }
            var discount = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Discount>();

            var response = new DiscountCompleteDTO(
                discount.Id,
                discount.Name,
                discount.DiscountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive,
                null,
                null,
                null
            );

            return Result<DiscountCompleteDTO>.Success(response);
        }
    }
}
