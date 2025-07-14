using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class ProductDiscountValidator : IBaseValidator<ProductDiscount>
    {
        private readonly ValidationBuilder _builder;

        public ProductDiscountValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<ProductDiscount> Validate(ProductDiscount entity)
        {
            var erros = _builder.Validate(entity);

            if (erros.Count != 0)
            {
                return Result<ProductDiscount>.Failure(erros);
            }

            return Result<ProductDiscount>.Success(entity);
        }
    }
}
