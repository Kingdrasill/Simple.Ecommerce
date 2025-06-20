using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class UserAddressValidator : IBaseValidator<UserAddress>
    {
        private readonly ValidationBuilder _builder;

        public UserAddressValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<UserAddress> Validate(UserAddress entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<UserAddress>.Failure(errors);
            }

            return Result<UserAddress>.Success(entity);
        }
    }
}
