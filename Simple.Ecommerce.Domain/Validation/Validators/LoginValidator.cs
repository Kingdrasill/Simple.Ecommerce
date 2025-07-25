using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class LoginValidator : IBaseValidator<Login>
    {
        private readonly ValidationBuilder _builder;

        public LoginValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<Login> Validate(Login entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<Login>.Failure(errors)
                : Result<Login>.Success(entity);
        }
    }
}
