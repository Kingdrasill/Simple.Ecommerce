using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class OrderValidator : IBaseValidator<Order>
    {
        private readonly ValidationBuilder _builder;

        public OrderValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<Order> Validate(Order entity)
        {
            var errors = _builder.Validate(entity);

            var addressValidator = new AddressValidator();
            var addressResult = addressValidator.Validate(entity.Address);

            if (addressResult.IsFailure) 
                errors.AddRange(addressResult.Errors!);

            if (entity.PaymentInformation is not null)
            {
                var paymentValidator = new PaymentInformationValidator();
                var paymentResult = paymentValidator.Validate(entity.PaymentInformation);

                if (paymentResult.IsFailure)
                    errors.AddRange(paymentResult.Errors!);
            }

            return errors.Count != 0
                ? Result<Order>.Failure(errors) 
                : Result<Order>.Success(entity);
        }
    }
}
