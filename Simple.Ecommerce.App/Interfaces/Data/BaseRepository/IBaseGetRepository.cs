using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseGetRepository<T> where T : class
    {
        Task<Result<T>> Get(int id, bool NoTracking = true);
    }
}
