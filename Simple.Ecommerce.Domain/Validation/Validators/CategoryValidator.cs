using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class CategoryValidator : IBaseValidator<Category>
    {
        private readonly ValidationBuilder _builder;

        public CategoryValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(Category.Name), typeof(Category).Name)
                .AddMaxLength(nameof(Category.Name), typeof(Category).Name, 30);
        }

        public Result<Category> Validate(Category entity)
        {
            var errors = _builder.Validate(entity);

            return errors.Count != 0
                ? Result<Category>.Failure(errors)
                : Result<Category>.Success(entity);
        }
    }
}
