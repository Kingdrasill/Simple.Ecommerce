using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

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

            if (erros.Count != 0)
            {
                return Result<Coupon>.Failure(erros);
            }

            return Result<Coupon>.Success(entity);
        }
    }
}
