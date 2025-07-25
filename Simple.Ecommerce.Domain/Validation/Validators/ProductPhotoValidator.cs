using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

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

            var photoValidator = new PhotoValidator();
            var photoResult = photoValidator.Validate(entity.Photo);

            if (photoResult.IsFailure)
                errors.AddRange(photoResult.Errors!);

            return errors.Count != 0
                ? Result<ProductPhoto>.Failure(errors)
                : Result<ProductPhoto>.Success(entity);
        }
    }
}
