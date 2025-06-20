namespace Simple.Ecommerce.App.Interfaces.Services.CredentialService
{
    public interface IEmailService
    {
        Task SendEmailVerification(string to, string token);
    }
}
