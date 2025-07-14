using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    internal class ProductPhotoValidator : IBaseValidator<ProductPhoto>
    {
        private readonly ValidationBuilder _builder;

        public ProductPhotoValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<ProductPhoto> Validate(ProductPhoto entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<ProductPhoto>.Failure(errors);
            }

            return Result<ProductPhoto>.Success(entity);
        }
    }
}
