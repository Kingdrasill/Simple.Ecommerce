namespace Simple.Ecommerce.App.Services.Generator
{
    public static class Generator
    {
        private static readonly Random _random = new();

        public static string GeneratePhoneVerificationCode(int length = 6)
        {
            return string.Concat(Enumerable.Range(0, length).Select(_ => _random.Next(0, 10)));
        }

        public static string GenerateCouponCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GenerateCouponCodeWithPrefix(string prefix, int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
            return $"{prefix}-{code}";
        }
    }
}
