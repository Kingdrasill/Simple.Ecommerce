using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

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

            if (errors.Count != 0)
            {
                return Result<OrderItem>.Failure(errors);
            }

            return Result<OrderItem>.Success(entity);
        }
    }
}
