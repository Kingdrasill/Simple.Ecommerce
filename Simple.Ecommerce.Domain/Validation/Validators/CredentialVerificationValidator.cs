using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class CredentialVerificationValidator : IBaseValidator<CredentialVerification>
    {
        private readonly ValidationBuilder _builder;

        public CredentialVerificationValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<CredentialVerification> Validate(CredentialVerification entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<CredentialVerification>.Failure(errors);
            }

            return Result<CredentialVerification>.Success(entity);
        }
    }
}
