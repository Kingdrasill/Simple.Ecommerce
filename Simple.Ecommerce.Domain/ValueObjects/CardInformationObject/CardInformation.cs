using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.CardInformationObject
{
    [Owned]
    public class CardInformation : ValueObject
    {
        public CardInformation() { }

        private CardInformation(string cardHolderName, string cardNumber, string expirationMonth, string expirationYear, CardFlag cardFlag, string last4Digits)
        {
            CardHolderName = cardHolderName;
            CardNumber = cardNumber;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            CardFlag = cardFlag;
            Last4Digits = last4Digits;
        }

        public Result<CardInformation> Create(string cardHolderName, string cardNumber, string expirationMonth, string expirationYear, CardFlag cardFlag, string last4Digits)
        {
            return new CardInformationValidator().Validate(new CardInformation(cardHolderName, cardNumber, expirationMonth, expirationYear, cardFlag, last4Digits));
        }

        public string CardHolderName { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpirationMonth { get; private set; }
        public string ExpirationYear { get; private set; }
        public CardFlag CardFlag { get; private set; }
        public string Last4Digits { get; private set; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return CardHolderName;
            yield return CardNumber;
            yield return ExpirationMonth;
            yield return ExpirationYear;
            yield return CardFlag;
            yield return Last4Digits;
        }
    }
}
