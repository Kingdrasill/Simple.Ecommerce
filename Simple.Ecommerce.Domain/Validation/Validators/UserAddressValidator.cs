using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

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

            var addressValidator = new AddressValidator();
            var addressResult = addressValidator.Validate(entity.Address);

            if (addressResult.IsFailure)
                errors.AddRange(addressResult.Errors!);

            return errors.Count != 0
                ? Result<UserAddress>.Failure(errors)
                : Result<UserAddress>.Success(entity);
        }
    }
}
