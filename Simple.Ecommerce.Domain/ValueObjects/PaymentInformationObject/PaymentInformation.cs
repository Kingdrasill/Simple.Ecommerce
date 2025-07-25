using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject
{
    [Owned]
    public class PaymentInformation : ValueObject
    {
        public PaymentInformation() { }

        public PaymentInformation(PaymentMethod paymentMethod, string? paymentName, string? paymentKey, string? expirationMonth, string? expirationYear, CardFlag? cardFlag, string? last4Digits)
        {
            PaymentMethod = paymentMethod;
            PaymentName = paymentName;
            PaymentKey = paymentKey;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            CardFlag = cardFlag;
            Last4Digits = last4Digits;
        }

        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentName { get; set; }
        public string? PaymentKey { get; set; }
        public string? ExpirationMonth { get; set; }
        public string? ExpirationYear { get; set; }
        public CardFlag? CardFlag { get; set; }
        public string? Last4Digits { get; set; }

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
