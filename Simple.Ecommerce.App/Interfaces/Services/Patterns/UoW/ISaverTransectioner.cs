using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW
{
    public interface ISaverTransectioner
    {
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
        Task<Result<bool>> SaveChanges();
    }
}
