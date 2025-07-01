using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericListRepository<T> where T : class
    {
        Task<Result<List<T>>> List(TesteDbContext context);
    }
}
