using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class ToggleActivationDiscountCommand : IToggleActivationDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ToggleActivationDiscountCommand(
            IDiscountRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int id, bool isActive)
        {
            var getDiscount = await _repository.Get(id);
            if (getDiscount.IsFailure)
            {
                return Result<bool>.Failure(getDiscount.Errors!);
            }
            var discount = getDiscount.GetValue();

            discount.SetActivity(isActive);
            if (discount.Validate() is { IsFailure: true } result)
            {
                return Result<bool>.Failure(result.Errors!);
            }

            var updateResult = await _repository.Update(discount);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Discount>();

            return Result<bool>.Success(true);
            
        }
    }
}
