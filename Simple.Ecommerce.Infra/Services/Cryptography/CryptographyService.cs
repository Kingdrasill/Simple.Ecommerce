using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Errors.BaseError;
using System.Security.Cryptography;
using System.Text;

namespace Simple.Ecommerce.Infra.Services.Cryptography
{
    public class CryptographyService : ICryptographyService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public CryptographyService(string keyBase64, string ivBase64)
        {
            _key = Convert.FromBase64String(keyBase64);
            _iv = Convert.FromBase64String(ivBase64);
        }

        public Result<string> HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return Result<string>.Success(hashedPassword);
        }

        public Result<bool> VerifyPassword(string password, string hashedPassword)
        {
            var result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            if (!result)
            {
                return Result<bool>.Failure(new List<Error> { new Error("CryptographyService.NotEqual", "Senhas diferentes!") });
            }

            return Result<bool>.Success(result);
        }

        public Result<string> Encrypt(string plainText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Result<string>.Success(Convert.ToBase64String(encryptedBytes));
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(new List<Error> { new Error("CryptographyService.Encrypt", ex.Message) });
            }
        }

        public Result<string> Decrypt(string encryptedBase64)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var decryptor = aes.CreateDecryptor();
                var encryptedBytes = Convert.FromBase64String(encryptedBase64);
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Result<string>.Success(Encoding.UTF8.GetString(decryptedBytes));
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(new List<Error> { new Error("CryptographyService.Decrypt", ex.Message) });
            }
        }
    }
}
