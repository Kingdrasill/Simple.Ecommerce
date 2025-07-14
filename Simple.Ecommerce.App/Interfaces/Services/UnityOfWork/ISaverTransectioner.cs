using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Services.UnityOfWork
{
    public interface ISaverTransectioner
    {
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
        Task<Result<bool>> SaveChanges();
    }
}
