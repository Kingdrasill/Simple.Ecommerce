using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Simple.Ecommerce.App.Interfaces.Services.Authentication;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Settings.JwtSettings;
using Simple.Ecommerce.Domain.ValueObjects.TokenObject;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simple.Ecommerce.Infra.Services.JwtToken
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtAuthenticationService(
            IOptions<JwtSettings> jwtSettings
        )
        {
            _jwtSettings = jwtSettings.Value;
        }

        public Result<Token> GenerateJwtToken(User user, Login login)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Credential", login.Credential),
                new Claim("LoginType", login.Type.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return Result<Token>.Success(new Token(new JwtSecurityTokenHandler().WriteToken(token), expiration));
        }
    }
}
