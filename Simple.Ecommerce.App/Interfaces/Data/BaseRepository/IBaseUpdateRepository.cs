using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseUpdateRepository<T> where T : class
    {
        Task<Result<T>> Update(T entity);
    }
}
