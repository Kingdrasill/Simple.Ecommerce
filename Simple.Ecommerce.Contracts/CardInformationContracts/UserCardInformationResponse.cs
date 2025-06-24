using Simple.Ecommerce.Domain.Enums.CardFlag;

namespace Simple.Ecommerce.Contracts.CardInformationContracts
{
    public record UserCardInformationResponse
    (
        string CardHolderName,
        string ExpirationMonth,
        string ExpirationYear,
        CardFlag CardFlag,
        string Last4Digits,
        int UserCardId
    );
}
