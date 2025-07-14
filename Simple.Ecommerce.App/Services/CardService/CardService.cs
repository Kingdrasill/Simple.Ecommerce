using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain;
using System.Text.RegularExpressions;

namespace Simple.Ecommerce.App.Services.CardService
{
    public class CardService : ICardService
    {
        public Result<CardFlag> GetCardFlag(string cardNumber)
        {
            return cardNumber switch
            {
                _ when cardNumber.StartsWith("4") => Result<CardFlag>.Success(CardFlag.Visa),
                _ when Regex.IsMatch(cardNumber, @"^5[1-5][0-9]{14}") => Result<CardFlag>.Success(CardFlag.MasterCard),
                _ when Regex.IsMatch(cardNumber, @"^3[47][0-9]{13}") => Result<CardFlag>.Success(CardFlag.AmericanExpress),
                _ when Regex.IsMatch(cardNumber, @"^(4011|4312|4389|4514|4576|5041|5066|5067|5090|6277|6362|6363|6364|6365|6366|6367|6368|6369)") => Result<CardFlag>.Success(CardFlag.Elo),
                _ when Regex.IsMatch(cardNumber, @"^60[0-9]{12,15}") => Result<CardFlag>.Success(CardFlag.Hipercard),
                _ when Regex.IsMatch(cardNumber, @"^3(?:0[0-5]|[68][0-9])") => Result<CardFlag>.Success(CardFlag.DinersClub),
                _ when Regex.IsMatch(cardNumber, @"^6(?:011|5[0-9]{2})") => Result<CardFlag>.Success(CardFlag.Discover),
                _ => Result<CardFlag>.Failure(new List<Error> { new("CardService.GetCardFlag.UnknownCardFlag", "Bandeira do cartão desconhecida.") })
            };
        }

        public Result<bool> IsValidCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return Result<bool>.Failure(new List<Error> { new("CardService.IsValidCardNumber.InvalidCardNumber", "O número do cartão não pode ser nulo ou vazio.") });
            }
            
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (!cardNumber.All(char.IsDigit))
                return Result<bool>.Failure(new List<Error> { new Error("CardService.InvalidFormat", "Card number must contain only digits") });

            return Result<bool>.Success(LuhnCheck(cardNumber));
        }

        private bool LuhnCheck(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }
    }
}
