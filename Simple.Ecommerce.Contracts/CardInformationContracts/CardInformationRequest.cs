namespace Simple.Ecommerce.Contracts.CardInformationContracts
{
    public record CardInformationRequest
    (
        string CardHolderName,
        string CardNumber,
        string ExpirationMonth,
        string ExpirationYear
    );
}
