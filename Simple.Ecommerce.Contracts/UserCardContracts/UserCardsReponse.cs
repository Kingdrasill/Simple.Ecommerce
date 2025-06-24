using Simple.Ecommerce.Contracts.CardInformationContracts;

namespace Simple.Ecommerce.Contracts.UserCardContracts
{
    public record UserCardsReponse
    (
        int UserId,
        string Name,
        List<UserCardInformationResponse> CardInformations
    );
}
