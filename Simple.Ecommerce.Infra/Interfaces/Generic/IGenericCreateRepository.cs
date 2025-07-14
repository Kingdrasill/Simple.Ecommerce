using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericCreateRepository<T> where T : class
    {
        public Task<Result<T>> Create(TesteDbContext context, T entity, bool skipSave = false);
    }
}
