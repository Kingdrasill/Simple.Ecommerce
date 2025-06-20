namespace Simple.Ecommerce.Contracts.LoginContracts
{
    public record AuthenticateResponse
    (
        string Token,
        DateTime Expiration
    );
}
