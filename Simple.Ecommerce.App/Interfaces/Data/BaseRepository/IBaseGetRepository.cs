using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseGetRepository<T> where T : class
    {
        Task<Result<T>> Get(int id, bool NoTracking = true);
    }
}
