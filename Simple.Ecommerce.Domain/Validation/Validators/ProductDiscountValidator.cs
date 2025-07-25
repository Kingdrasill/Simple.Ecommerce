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

            return erros.Count != 0
                ? Result<ProductDiscount>.Failure(erros)
                : Result<ProductDiscount>.Success(entity);
        }
    }
}
