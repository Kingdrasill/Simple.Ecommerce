namespace Simple.Ecommerce.Domain.Settings.JwtSettings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresInHours { get; set; }
    }
}
