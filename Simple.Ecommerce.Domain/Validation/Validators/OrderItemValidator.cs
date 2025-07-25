using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class OrderItemValidator : IBaseValidator<OrderItem>
    {
        private readonly ValidationBuilder _builder;

        public OrderItemValidator()
        {
            _builder = new ValidationBuilder()
                .AddNegativeValueDecimal(nameof(OrderItem.Price), typeof(OrderItem).Name)
                .AddNegativeValueInt(nameof(OrderItem.Quantity), typeof(OrderItem).Name);
        }

        public Result<OrderItem> Validate(OrderItem entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<OrderItem>.Failure(errors)
                : Result<OrderItem>.Success(entity);
        }
    }
}
