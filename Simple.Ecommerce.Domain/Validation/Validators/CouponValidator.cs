using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class CouponValidator : IBaseValidator<Coupon>
    {
        private readonly ValidationBuilder _builder;

        public CouponValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<Coupon> Validate(Coupon entity)
        {
            var erros = _builder.Validate(entity);

            return erros.Count != 0
                ? Result<Coupon>.Failure(erros)
                : Result<Coupon>.Success(entity);
        }
    }
}
