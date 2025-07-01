using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.Objects;

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

            if (errors.Count != 0)
            {
                return Result<ProductCategory>.Failure(errors);
            }

            return Result<ProductCategory>.Success(entity);
        }
    }
}
