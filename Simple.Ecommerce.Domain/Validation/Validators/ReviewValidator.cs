using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class ReviewValidator : IBaseValidator<Review>
    {
        private readonly ValidationBuilder _builder;

        public ReviewValidator()
        {
            _builder = new ValidationBuilder()
                .AddOutOfRange(nameof(Review.Score), typeof(Review).Name, 0, 5)
                .AddRule(nameof(Review.Comment), s => (string)s is { Length: > 200 }, $"{typeof(Review).Name}.MaxLength", "Não pode ter mais de 200 caracteres"); ;
        }

        public Result<Review> Validate(Review entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<Review>.Failure(errors);
            }

            return Result<Review>.Success(entity);
        }
    }
}
