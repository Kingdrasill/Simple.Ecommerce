using System.Security.Cryptography;
using System.Text;

namespace Simple.Ecommerce.App.Services.Generator
{
    public static class Generator
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GeneratePhoneVerificationCode(int length = 6)
        {
            return GenerateDigits(length);
        }

        public static string GenerateCouponCode(int length = 12)
        {
            return GenerateRandomString(chars, length);
        }

        public static string GenerateCouponCodeWithPrefix(string prefix, int length = 12)
        {
            var code = GenerateRandomString(chars, length);
            return $"{prefix}-{code}";
        }

        private static string GenerateDigits(int length) 
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            var result = new StringBuilder(length);
            foreach (var b in bytes)
                result.Append(b % 10);
            return result.ToString();
        }

        private static string GenerateRandomString(string charset, int length)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            var result = new StringBuilder(length);
            foreach (var b in bytes)
                result.Append(charset[b % charset.Length]);
            return result.ToString();
        }
    }
}
