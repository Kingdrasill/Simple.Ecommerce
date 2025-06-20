using Simple.Ecommerce.Domain.Enums.Crendetial;

namespace Simple.Ecommerce.Contracts.LoginContracts
{
    public record LoginRequest
    (
        string Credential,
        string Password,
        CredentialType Type,
        int UserId,
        int Id = 0
    );
}
