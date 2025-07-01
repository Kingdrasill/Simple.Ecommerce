using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericUpdateRepository<T> : IGenericUpdateRepository<T> where T : class
    {
        public virtual Task<Result<T>> Update(TesteDbContext context, T entity)
        {
            context.Set<T>().Update(entity);
            return Task.FromResult(Result<T>.Success(entity));
        }
    }
}
