using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Interfaces.BaseEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericGetRepository<T> : IGenericGetRepository<T> where T : class, IBaseEntity, new()
    {
        public virtual async Task<Result<T>> Get(TesteDbContext context, int id, bool NoTracking = true)
        {
            IQueryable<T> query = context.Set<T>();
            
            if (NoTracking)
            {
                query = query.AsNoTracking();
            }

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id && !e.Deleted);
            
            if (entity is null)
                return Result<T>.Failure(new List<Error> { new Error("NotFound", "Entidade não foi encontrada!") });

            return Result<T>.Success(entity);
        }
    }
}
