using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.App.Services.Generator;
using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class CreateBatchCouponsDiscountCommand : ICreateBatchCouponsDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly ICouponRepository _couponRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateBatchCouponsDiscountCommand(
            IDiscountRepository repository, 
            ICouponRepository couponRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _couponRepository = couponRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<CouponResponse>>> Execute(CouponBatchRequest request)
        {
            var getDiscount = await _repository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<List<CouponResponse>>.Failure(getDiscount.Errors!);
            }

            var response = new List<CouponResponse>();
            for (int i = 0; i <request.Quantity; i++)
            {
                string code;
                do
                {
                    code = string.IsNullOrEmpty(request.Prefix) ? Generator.GenerateCouponCode() : Generator.GenerateCouponCodeWithPrefix(request.Prefix);
                    var usedCode = await _couponRepository.GetByCode(code);
                    if (usedCode.IsFailure)
                        break;
                } 
                while (true);

                var instance = new Coupon().Create(
                    0,
                    code, 
                    request.ExpirationAt,
                    request.DiscountId
                );
                if (instance.IsFailure)
                {
                    return Result<List<CouponResponse>>.Failure(instance.Errors!);
                }

                var createResult = await _couponRepository.Create(instance.GetValue());
                if (createResult.IsFailure)
                {
                    return Result<List<CouponResponse>>.Failure(createResult.Errors!);
                }

                var coupon = createResult.GetValue();

                response.Add(new CouponResponse(
                    coupon.Id,
                    coupon.Code,
                    coupon.IsUsed,
                    coupon.CreatedAt,
                    coupon.ExpirationAt,
                    coupon.UsedAt,
                    coupon.DiscountId
                ));
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Coupon>();

            return Result<List<CouponResponse>>.Success(response);
        }
    }
}
