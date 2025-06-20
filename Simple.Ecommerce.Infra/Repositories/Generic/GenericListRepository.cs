using Simple.Ecommerce.Domain.Interfaces.BaseEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericListRepository<T> : IGenericListRepository<T> where T : class, IBaseEntity
    {
        public virtual async Task<Result<List<T>>> List(TesteDbContext context)
        {
            var listEntity = await context.Set<T>()
                .Where(e => !e.Deleted)
                .ToListAsync();

            return Result<List<T>>.Success(listEntity);
        }
    }
}
    