namespace Simple.Ecommerce.App.Interfaces.Services.CredentialService
{
    public interface ISmsService
    {
        Task SendVerificationCode(string phoneNumber, string code);
    }
}
