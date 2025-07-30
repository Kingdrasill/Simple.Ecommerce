using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericDetachRepository<T> : IGenericDetachRepository<T> where T : class
    {
        public void Detach(TesteDbContext context, T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }
    }
}
