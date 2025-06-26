using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Services.CardService
{
    public interface ICardService
    {
        Result<CardFlag> GetCardFlag(string cardNumber);
        Result<bool> IsValidCardNumber(string cardNumber);
    }
}
