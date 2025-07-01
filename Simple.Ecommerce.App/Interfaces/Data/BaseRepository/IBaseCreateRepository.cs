using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseCreateRepository<T> where T : class
    {
        Task<Result<T>> Create(T entity);
    }
}
