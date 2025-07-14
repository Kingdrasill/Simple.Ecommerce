namespace Simple.Ecommerce.App.Interfaces.Services.UnityOfWork
{
    public interface IBaseUnityOfWork
    {
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
    }
}
