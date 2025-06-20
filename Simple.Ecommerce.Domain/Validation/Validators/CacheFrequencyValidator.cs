using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class CacheFrequencyValidator : IBaseValidator<CacheFrequency>
    {
        private readonly ValidationBuilder _builder;

        public CacheFrequencyValidator()
        {
            _builder = new ValidationBuilder()
                .AddNegativeValueInt(nameof(CacheFrequency.Frequency), typeof(CacheFrequency).Name);
        }

        public Result<CacheFrequency> Validate(CacheFrequency entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<CacheFrequency>.Failure(errors);
            }

            return Result<CacheFrequency>.Success(entity);
        }
    }
}
