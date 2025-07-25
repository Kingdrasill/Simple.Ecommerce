using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class DiscountValidator : IBaseValidator<Discount>
    {
        private readonly ValidationBuilder _builder;

        public DiscountValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(Discount.Name), typeof(Discount).Name)
                .AddMaxLength(nameof(Discount.Name), typeof(Discount).Name, 30)
                .AddEntityRule(
                    nameof(Discount.Value),
                    e =>
                    {
                        var d = (Discount)e;
                        return d.DiscountValueType != null &&
                               d.Value != null &&
                               d.DiscountValueType == DiscountValueType.Percentage &&
                               (d.Value < 0 || d.Value > 1);
                    },
                    $"{typeof(Discount).Name}.OutOfRange",
                    "Para descontos por porcentagem, precisa estar entre 0 e 1!"
                )
                .AddEntityRule(
                    nameof(Discount.Value),
                    e =>
                    {
                        var d = (Discount)e;
                        return d.DiscountValueType != null &&
                               d.Value != null &&
                               d.DiscountValueType == DiscountValueType.FixedAmount &&
                               d.Value < 0;
                    },
                    $"{typeof(Discount).Name}.NegativeValue",
                    "Para descontos por valor fixos, não pode ser negativo!"
                )
                .AddInvalidDateRange<Discount>($"[{nameof(Discount.ValidFrom)},{nameof(Discount.ValidTo)}]", typeof(Discount).Name, d => d.ValidFrom, d => d.ValidTo);
        }

        public Result<Discount> Validate(Discount entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<Discount>.Failure(errors)
                : Result<Discount>.Success(entity);
        }
    }
}
