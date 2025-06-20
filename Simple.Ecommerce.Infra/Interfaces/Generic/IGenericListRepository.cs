using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericListRepository<T> where T : class
    {
        Task<Result<List<T>>> List(TesteDbContext context);
    }
}
