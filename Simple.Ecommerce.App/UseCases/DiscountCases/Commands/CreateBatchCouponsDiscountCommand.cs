using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.Generator;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class CreateBatchCouponsDiscountCommand : ICreateBatchCouponsDiscountCommand
    {
        private readonly ICreateBatchCouponUnitOfWork _createBatchCouponUoW;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateBatchCouponsDiscountCommand(
            ICreateBatchCouponUnitOfWork createBatchCouponUoW,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _createBatchCouponUoW = createBatchCouponUoW;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<List<CouponResponse>>> Execute(CouponBatchRequest request)
        {
            await _createBatchCouponUoW.BeginTransaction();
            try
            {
                var getDiscount = await _createBatchCouponUoW.Discounts.Get(request.DiscountId);
                if (getDiscount.IsFailure)
                {
                    throw new ResultException(getDiscount.Errors!);
                }

                var response = new List<CouponResponse>();
                for (int i = 0; i < request.Quantity; i++)
                {
                    string code;
                    do
                    {
                        code = string.IsNullOrEmpty(request.Prefix) ? Generator.GenerateCouponCode() : Generator.GenerateCouponCodeWithPrefix(request.Prefix);
                        var usedCode = await _createBatchCouponUoW.Coupons.GetByCode(code);
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
                        throw new ResultException(instance.Errors!);
                    }

                    var createResult = await _createBatchCouponUoW.Coupons.Create(instance.GetValue(), true);
                    if (createResult.IsFailure)
                    {
                        throw new ResultException(createResult.Errors!);
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

                await _createBatchCouponUoW.Commit();

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<Coupon>();

                return Result<List<CouponResponse>>.Success(response);
            }
            catch (ResultException rex)
            {
                await _createBatchCouponUoW.Rollback();
                return Result<List<CouponResponse>>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _createBatchCouponUoW.Rollback();
                return Result<List<CouponResponse>>.Failure(new List<Error> { new("CreateBatchCouponsDiscountCommand.Unknown", ex.Message) });
            }
        }
    }
}
