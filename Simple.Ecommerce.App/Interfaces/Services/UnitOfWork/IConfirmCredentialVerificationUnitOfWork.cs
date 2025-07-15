using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IConfirmCredentialVerificationUnitOfWork : IBaseUnitOfWork
    {
        ICredentialVerificationRepository CredentialVerifications { get; }
        ILoginRepository Logins { get; }
    }
}
