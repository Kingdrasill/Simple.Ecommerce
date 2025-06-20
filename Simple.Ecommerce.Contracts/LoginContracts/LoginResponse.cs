using Simple.Ecommerce.Domain.Enums.Crendetial;

namespace Simple.Ecommerce.Contracts.LoginContracts
{
    public record LoginResponse
    (
        int Id,
        int UserId,
        string Credential,
        string Password,
        CredentialType Type,
        bool IsVerified
    );
}
