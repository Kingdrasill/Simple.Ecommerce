using Simple.Ecommerce.Contracts.CardInformationContracts;

namespace Simple.Ecommerce.Contracts.UserCardContracts
{
    public record UserCardRequest
    (
        int UserId,
        CardInformationRequest CardInformation
    );
}
