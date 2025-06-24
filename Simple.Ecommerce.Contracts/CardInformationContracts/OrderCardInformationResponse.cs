using Simple.Ecommerce.Domain.Enums.CardFlag;

namespace Simple.Ecommerce.Contracts.CardInformationContracts
{
    public record OrderCardInformationResponse
    (
        string CardHolderName,
        string ExpirationMonth,
        string ExpirationYear,
        CardFlag CardFlag,
        string Last4Digits
    );
}
