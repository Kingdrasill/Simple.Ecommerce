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

            var paymentValidator = new PaymentInformationValidator();
            var paymentResult = paymentValidator.Validate(entity.PaymentInformation);

            if (paymentResult.IsFailure)
                erros.AddRange(paymentResult.Errors!);

            return erros.Count != 0
                ? Result<UserPayment>.Failure(erros)
                : Result<UserPayment>.Success(entity);
        }
    }
}
