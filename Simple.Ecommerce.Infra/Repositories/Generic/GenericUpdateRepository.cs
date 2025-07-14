using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericUpdateRepository<T> : IGenericUpdateRepository<T> where T : class
    {
        public virtual Task<Result<T>> Update(TesteDbContext context, T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.Set<T>().Update(entity);
            return Task.FromResult(Result<T>.Success(entity));
        }
    }
}
