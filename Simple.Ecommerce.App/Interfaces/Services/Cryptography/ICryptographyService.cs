using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Services.Cryptography
{
    public interface ICryptographyService
    {
        Result<string> HashPassword(string password);

        Result<bool> VerifyPassword(string password, string hashedPassword);
        Result<string> Encrypt(string plainText);
        Result<string> Decrypt(string encryptedBase64);
    }
}
