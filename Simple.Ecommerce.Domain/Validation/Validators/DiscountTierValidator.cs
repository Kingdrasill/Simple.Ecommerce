using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class DiscountTierValidator : IBaseValidator<DiscountTier>
    {
        private readonly ValidationBuilder _builder;

        public DiscountTierValidator() 
        { 
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(DiscountTier.Name), typeof(DiscountTier).Name)
                .AddMaxLength(nameof(DiscountTier.Name), typeof(DiscountTier).Name, 30)
                .AddNegativeValueInt(nameof(DiscountTier.MinQuantity), typeof(DiscountTier).Name)
                .AddNegativeValueDecimal(nameof(DiscountTier.Value), typeof(DiscountTier).Name);
        }

        public Result<DiscountTier> Validate(DiscountTier entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<DiscountTier>.Failure(errors);
            }

            return Result<DiscountTier>.Success(entity);
        }
    }
}
