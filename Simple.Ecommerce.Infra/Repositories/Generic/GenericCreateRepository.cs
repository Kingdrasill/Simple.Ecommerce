using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericCreateRepository<T> : IGenericCreateRepository<T> where T : class
    {
        public virtual async Task<Result<T>> Create(TesteDbContext context, T entity)
        {
            await context.Set<T>().AddAsync(entity);
            return Result<T>.Success(entity);
        }
    }
}
