using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericUpdateRepository<T> where T : class
    {
        Task<Result<T>> Update(TesteDbContext context, T entity);
    }
}
