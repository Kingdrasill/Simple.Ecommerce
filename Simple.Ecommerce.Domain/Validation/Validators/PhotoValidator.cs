using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class PhotoValidator : IBaseValidator<Photo>
    {
        private readonly ValidationBuilder _builder;

        public PhotoValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(Photo.FileName), typeof(Photo).Name);
        }

        public Result<Photo> Validate(Photo photo)
        {
            var erros = _builder.Validate(photo);

            return erros.Count != 0
                ? Result<Photo>.Failure(erros)
                : Result<Photo>.Success(photo);
        }
    }
}
