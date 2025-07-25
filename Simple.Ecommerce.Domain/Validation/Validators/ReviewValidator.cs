using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

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

            return errors.Count != 0
                ? Result<Review>.Failure(errors)
                : Result<Review>.Success(entity);
        }
    }
}
