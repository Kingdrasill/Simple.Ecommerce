using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

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

            if (erros.Count != 0)
            {
                return Result<Photo>.Failure(erros);
            }

            return Result<Photo>.Success(photo);
        }
    }
}
