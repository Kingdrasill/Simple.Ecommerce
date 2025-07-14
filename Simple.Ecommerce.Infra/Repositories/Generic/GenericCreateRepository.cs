using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericCreateRepository<T> : IGenericCreateRepository<T> where T : class
    {
        public virtual async Task<Result<T>> Create(TesteDbContext context, T entity, bool skipSave = false)
        {
            await context.Set<T>().AddAsync(entity);
            if (!skipSave)
                await context.SaveChangesAsync();
            return Result<T>.Success(entity);
        }
    }
}
