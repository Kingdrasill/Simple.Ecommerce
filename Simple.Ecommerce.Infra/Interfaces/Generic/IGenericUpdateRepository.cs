using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericUpdateRepository<T> where T : class
    {
        Task<Result<T>> Update(TesteDbContext context, T entity, bool skipSave = false);
    }
}
