using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.ValueObjects.TokenObject;

namespace Simple.Ecommerce.App.Interfaces.Services.Authentication
{
    public interface IJwtAuthenticationService
    {
        Result<Token> GenerateJwtToken(User user, Login login);
    }
}
