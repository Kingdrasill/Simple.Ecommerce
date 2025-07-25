using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class ProductValidator : IBaseValidator<Product>
    {
        private readonly ValidationBuilder _builder;

        public ProductValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(Product.Name), typeof(Product).Name)
                .AddMaxLength(nameof(Product.Name), typeof(Product).Name, 50)
                .AddNegativeValueDecimal(nameof(Product.Price), typeof(Product).Name)
                .AddEmptyValue(nameof(Product.Description), typeof(Product).Name)
                .AddMaxLength(nameof(Product.Description), typeof(Product).Name, 100)
                .AddNegativeValueInt(nameof(Product.Stock), typeof(Product).Name);
        }

        public Result<Product> Validate(Product entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<Product>.Failure(errors)
                : Result<Product>.Success(entity);
        }
    }
}
