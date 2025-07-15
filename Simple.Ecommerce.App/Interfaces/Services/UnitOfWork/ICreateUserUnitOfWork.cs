using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface ICreateUserUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
        ILoginRepository Logins { get; }
        ICredentialVerificationRepository CredentialVerifications { get; }
    }
}
