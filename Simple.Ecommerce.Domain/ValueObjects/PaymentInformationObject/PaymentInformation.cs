using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject
{
    [Owned]
    public class PaymentInformation : ValueObject
    {
        public PaymentInformation() { }

        private PaymentInformation(PaymentMethod paymentMethod, string? paymentName, string? paymentKey, string? expirationMonth, string? expirationYear, CardFlag? cardFlag, string? last4Digits)
        {
            PaymentName = paymentName;
            PaymentKey = paymentKey;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            CardFlag = cardFlag;
            Last4Digits = last4Digits;
        }

        public Result<PaymentInformation> Create(PaymentMethod paymentMethod, string? paymentName, string? paymentKey, string? expirationMonth, string? expirationYear, CardFlag? cardFlag, string? last4Digits)
        {
            return new PaymentInformationValidator().Validate(new PaymentInformation(paymentMethod, paymentName, paymentKey, expirationMonth, expirationYear, cardFlag, last4Digits));
        }

        public PaymentMethod PaymentMethod { get; private set; }
        public string? PaymentName { get; private set; }
        public string? PaymentKey { get; private set; }
        public string? ExpirationMonth { get; private set; }
        public string? ExpirationYear { get; private set; }
        public CardFlag? CardFlag { get; private set; }
        public string? Last4Digits { get; private set; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return PaymentMethod;
            yield return PaymentName;
            yield return PaymentKey;
            yield return ExpirationMonth;
            yield return ExpirationYear;
            yield return CardFlag;
            yield return Last4Digits;
        }
    }
}
