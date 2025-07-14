using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericGetRepository<T> where T : class
    {
        Task<Result<T>> Get(TesteDbContext context, int id, bool NoTracking = true);
    }
}
