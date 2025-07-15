namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IBaseUnitOfWork
    {
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
    }
}
