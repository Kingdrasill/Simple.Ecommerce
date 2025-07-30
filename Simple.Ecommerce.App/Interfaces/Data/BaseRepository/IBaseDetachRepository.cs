namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseDetachRepository<T> where T : class
    {
        void Detach(T entity);
    }
}
