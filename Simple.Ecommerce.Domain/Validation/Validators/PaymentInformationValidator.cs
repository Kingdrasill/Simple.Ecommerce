using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class PaymentInformationValidator : IBaseValidator<PaymentInformation>
    {
        private readonly ValidationBuilder _builder;

        public PaymentInformationValidator()
        {
            _builder = new ValidationBuilder()
                .AddMaxLength(nameof(PaymentInformation.PaymentName), typeof(PaymentInformation).Name, 50)
                .AddMaxLength(nameof(PaymentInformation.ExpirationMonth), typeof(PaymentInformation).Name, 2)
                .AddMaxLength(nameof(PaymentInformation.ExpirationYear), typeof(PaymentInformation).Name, 5);
        }

        public Result<PaymentInformation> Validate(PaymentInformation entity)
        {
            var erros = _builder.Validate(entity);

            return erros.Count != 0
                ? Result<PaymentInformation>.Failure(erros)
                : Result<PaymentInformation>.Success(entity);
        }
    }
}
