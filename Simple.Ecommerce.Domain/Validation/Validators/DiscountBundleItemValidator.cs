using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class DiscountBundleItemValidator : IBaseValidator<DiscountBundleItem>
    {
        private readonly ValidationBuilder _builder;

        public DiscountBundleItemValidator()
        {
            _builder = new ValidationBuilder()
                .AddNegativeValueInt(nameof(DiscountBundleItem.Quantity), typeof(DiscountBundleItem).Name);
        }

        public Result<DiscountBundleItem> Validate(DiscountBundleItem entity)
        {
            var erros = _builder.Validate(entity);

            return erros.Count != 0
                ? Result<DiscountBundleItem>.Failure(erros)
                : Result<DiscountBundleItem>.Success(entity);
        }
    }
}
