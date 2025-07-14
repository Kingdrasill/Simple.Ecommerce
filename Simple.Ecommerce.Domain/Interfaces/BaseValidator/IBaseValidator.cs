using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Domain.Interfaces.BaseValidator
{
    public interface IBaseValidator<T> where T : class
    {
        public Result<T> Validate(T entity);
    }
}
