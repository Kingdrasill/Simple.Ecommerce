using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseUpdateRepository<T> where T : class
    {
        Task<Result<T>> Update(T entity);
    }
}
