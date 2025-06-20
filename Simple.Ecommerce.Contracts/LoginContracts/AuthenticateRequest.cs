using Simple.Ecommerce.Domain.Enums.Crendetial;

namespace Simple.Ecommerce.Contracts.LoginContracts
{
    public record AuthenticateRequest
    (
        string Credential,
        string Password,
        CredentialType Type
    );
}
