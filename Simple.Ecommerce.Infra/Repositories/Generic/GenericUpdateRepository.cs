using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericUpdateRepository<T> : IGenericUpdateRepository<T> where T : class
    {
        public virtual async Task<Result<T>> Update(TesteDbContext context, T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
            return Result<T>.Success(entity);
        }
    }
}
