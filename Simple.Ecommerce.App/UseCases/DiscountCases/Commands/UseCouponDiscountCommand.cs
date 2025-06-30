using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UseCouponDiscountCommand : IUseCouponDiscountCommand
    {
        private readonly ICouponRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UseCouponDiscountCommand(
            ICouponRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(string code)
        {
            var getCoupon = await _repository.GetByCode(code);
            if (getCoupon.IsFailure)
            {
                return Result<bool>.Failure(getCoupon.Errors!);
            }

            var coupon = getCoupon.GetValue();

            if (coupon.ExpirationAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure(new List<Error> { new("UseCouponDiscountCommand.DateExpired.ExpirationAt", "O cupom já expirado!") });
            }

            if (coupon.IsUsed)
            {
                return Result<bool>.Failure(new List<Error> { new("UseCouponDiscountCommand.AlredayUsed", "O cupom já foi usado!") });
            }

            coupon.SetAsUsed();

            var updateResult = await _repository.Update(coupon);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Coupon>();

            return Result<bool>.Success(true);
        }
    }
}
