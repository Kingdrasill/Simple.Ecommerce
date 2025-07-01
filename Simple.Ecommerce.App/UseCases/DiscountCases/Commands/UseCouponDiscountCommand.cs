using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UseCouponDiscountCommand : IUseCouponDiscountCommand
    {
        private readonly ICouponRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UseCouponDiscountCommand(
            ICouponRepository repository, 
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
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

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<bool>.Failure(commit.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Coupon>();

            return Result<bool>.Success(true);
        }
    }
}
