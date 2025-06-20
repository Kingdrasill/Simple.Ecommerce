namespace Simple.Ecommerce.App.Interfaces.ReadData.BaseReadRepository
{
    public interface IBaseSaveReadRepository<T> where T : class
    {
        Task Save(T model);
    }
}
