using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class OrderDiscountValidator : IBaseValidator<OrderDiscount>
    {
        private readonly ValidationBuilder _builder;

        public OrderDiscountValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<OrderDiscount> Validate(OrderDiscount entity)
        {
            var erros = _builder.Validate(entity);

            if (erros.Count != 0)
            {
                return Result<OrderDiscount>.Failure(erros);
            }

            return Result<OrderDiscount>.Success(entity);
        }
    }
}
