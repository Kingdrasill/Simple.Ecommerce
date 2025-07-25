using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

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

            return errors.Count != 0
                ? Result<CredentialVerification>.Failure(errors)
                : Result<CredentialVerification>.Success(entity);
        }
    }
}
