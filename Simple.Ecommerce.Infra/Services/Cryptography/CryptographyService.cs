using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Infra.Services.Cryptography
{
    public class CryptographyService : ICryptographyService
    {
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
    }
}
