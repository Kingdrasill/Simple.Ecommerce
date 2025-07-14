using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class UpdateCouponDiscountCommand : IUpdateCouponDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly ICouponRepository _couponRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateCouponDiscountCommand(
            IDiscountRepository repository, 
            ICouponRepository couponRepository,
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _couponRepository = couponRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<CouponResponse>> Execute(CouponRequest request)
        {
            var getCoupon = await _couponRepository.Get(request.Id);
            if (getCoupon.IsFailure)
            {
                return Result<CouponResponse>.Failure(getCoupon.Errors!);
            }

            var getDiscount = await _repository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<CouponResponse>.Failure(getDiscount.Errors!);
            }

            var instance = new Coupon().Create(
                request.Id,
                request.Code,
                request.ExpirationAt,
                request.DiscountId,
                request.IsUsed,
                request.UsedAt,
                request.CreatedAt
            );
            if (instance.IsFailure)
            {
                return Result<CouponResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _couponRepository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<CouponResponse>.Failure(updateResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<CouponResponse>.Failure(commit.Errors!);
            }

            var coupon = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Coupon>();

            var response = new CouponResponse(
                coupon.Id,
                coupon.Code,
                coupon.IsUsed,
                coupon.CreatedAt,
                coupon.ExpirationAt,
                coupon.UsedAt,
                coupon.DiscountId
            );

            return Result<CouponResponse>.Success(response);
        }
    }
}
