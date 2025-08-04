using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation;
using Simple.Ecommerce.Contracts.OrderContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ChangeDiscountOrderCommand : IChangeDiscountOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangeDiscountOrderCommand(
            IOrderRepository repository, 
            IDiscountRepository discountRepository,
            ICouponRepository couponRepository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountRepository = discountRepository;
            _couponRepository = couponRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(OrderDiscountRequest request)
        {
            var getOrder = await _repository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<bool>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not OrderLock.Unlock)
            {
                return Result<bool>.Failure(new List<Error> { new("ChangeDiscountOrderCommand.OrderLocked", "Alterações que afetem o preço do pedido estão bloqueadas para este pedido.") });
            }

            var couponCode = request.CouponCode;
            var discountId = request.DiscountId;

            int? couponId = null;
            var getCoupon = couponCode is null ? null : await _couponRepository.GetByCode(couponCode);
            if (getCoupon is not null)
            {
                if (getCoupon.IsFailure)
                {
                    return Result<bool>.Failure(getCoupon.Errors!);
                }
                var coupon = getCoupon.GetValue();

                if (coupon.IsUsed)
                {
                    return Result<bool>.Failure(new List<Error> { new("ChangeDiscountOrderCommand.AlreadyUsed", "O cupom passado já foi usado!") });
                }

                couponId = coupon.Id;
                discountId = coupon.DiscountId;
            }
            var getDiscount = discountId is null ? null : await _discountRepository.Get(discountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                {
                    return Result<bool>.Failure(getDiscount.Errors!);
                }

                var simpleValidation = SimpleDiscountValidation.Validate(getDiscount.GetValue(), DiscountScope.Order, "ChangeDiscountOrderCommand", null);
                if (simpleValidation.IsFailure)
                {
                    return Result<bool>.Failure(simpleValidation.Errors!);
                }
            }

            order.UpdateDiscount(couponId, discountId);
            order.UpdateStatus("Altered", order.OrderLock);
            if (order.Validate() is { IsFailure:  true } result)
            {
                return Result<bool>.Failure(result.Errors!);
            }

            var updateResult = await _repository.Update(order);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            return Result<bool>.Success(true);
        }
    }
}
