using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class UserPaymentValidator : IBaseValidator<UserPayment>
    {
        private readonly ValidationBuilder _builder;

        public UserPaymentValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<UserPayment> Validate(UserPayment entity)
        {
            var erros = _builder.Validate(entity);

            if (erros.Count != 0)
            {
                return Result<UserPayment>.Failure(erros);
            }

            return Result<UserPayment>.Success(entity);
        }
    }
}
