using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class UserCardValidator : IBaseValidator<UserCard>
    {
        private readonly ValidationBuilder _builder;

        public UserCardValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<UserCard> Validate(UserCard entity)
        {
            var erros = _builder.Validate(entity);

            if (erros.Count != 0)
            {
                return Result<UserCard>.Failure(erros);
            }

            return Result<UserCard>.Success(entity);
        }
    }
}
