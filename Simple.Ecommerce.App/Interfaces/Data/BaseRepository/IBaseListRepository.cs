using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseListRepository<T> where T : class
    {
        Task<Result<List<T>>> List();
    }
}
