using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.Objects;

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

            if (errors.Count != 0)
            {
                return Result<Login>.Failure(errors);
            }

            return Result<Login>.Success(entity);
        }
    }
}
