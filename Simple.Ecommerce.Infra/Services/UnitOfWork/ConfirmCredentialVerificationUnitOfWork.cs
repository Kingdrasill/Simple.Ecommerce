using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class ConfirmCredentialVerificationUnitOfWork : BaseUnitOfWork, IConfirmCredentialVerificationUnitOfWork
    {
        public ICredentialVerificationRepository CredentialVerifications { get; }
        public ILoginRepository Logins { get; }

        public ConfirmCredentialVerificationUnitOfWork(
            TesteDbContext context, 
            ICredentialVerificationRepository credentialVerifications, 
            ILoginRepository logins
        ) : base(context)
        {
            CredentialVerifications = credentialVerifications;
            Logins = logins;
        }
    }
}
