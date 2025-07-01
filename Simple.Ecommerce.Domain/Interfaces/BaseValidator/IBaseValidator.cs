using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Domain.Interfaces.BaseValidator
{
    public interface IBaseValidator<T> where T : class
    {
        public Result<T> Validate(T entity);
    }
}
