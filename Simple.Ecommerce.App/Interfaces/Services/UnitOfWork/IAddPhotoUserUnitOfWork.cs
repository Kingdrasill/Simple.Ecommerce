using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IAddPhotoUserUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
    }
}
