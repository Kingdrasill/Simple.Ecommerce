using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericUpdateRepository<T> : IGenericUpdateRepository<T> where T : class
    {
        public async virtual Task<Result<T>> Update(TesteDbContext context, T entity, bool skipSave = false)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.Set<T>().Update(entity);
            if (!skipSave)
                await context.SaveChangesAsync();
            return Result<T>.Success(entity);
        }
    }
}
