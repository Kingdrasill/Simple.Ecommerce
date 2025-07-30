namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IBaseUnitOfWork
    {
        Task BeginTransaction();
        Task SaveChanges();
        Task Commit();
        Task Rollback();
    }
}
