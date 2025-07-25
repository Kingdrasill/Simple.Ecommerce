using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class ProductCategoryValidator : IBaseValidator<ProductCategory>
    {
        private readonly ValidationBuilder _builder;

        public ProductCategoryValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<ProductCategory> Validate(ProductCategory entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<ProductCategory>.Failure(errors)
                : Result<ProductCategory>.Success(entity);
        }
    }
}
