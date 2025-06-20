using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class OrderValidator : IBaseValidator<Order>
    {
        private readonly ValidationBuilder _builder;

        public OrderValidator()
        {
            _builder = new ValidationBuilder()
                .AddNegativeValueDecimal(nameof(Order.TotalPrice), typeof(Order).Name)
                .AddEmptyValue(nameof(Order.OrderType), typeof(Order).Name)
                .AddMaxLength(nameof(Order.OrderType), typeof(Order).Name, 5)
                .AddEmptyValue(nameof(Order.PaymentMethod), typeof(Order).Name)
                .AddMaxLength(nameof(Order.PaymentMethod), typeof(Order).Name, 20);
        }

        public Result<Order> Validate(Order entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<Order>.Failure(errors);
            }

            return Result<Order>.Success(entity);
        }
    }
}
