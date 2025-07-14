using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class DiscountValidator : IBaseValidator<Discount>
    {
        private readonly ValidationBuilder _builder;

        public DiscountValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(Discount.Name), typeof(Discount).Name)
                .AddMaxLength(nameof(Discount.Name), typeof(Discount).Name, 30);
        }

        public Result<Discount> Validate(Discount entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<Discount>.Failure(errors);
            }

            return Result<Discount>.Success(entity);
        }
    }
}
