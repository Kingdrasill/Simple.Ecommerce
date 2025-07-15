using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class CreateUserUnitOfWork : BaseUnitOfWork, ICreateUserUnitOfWork
    {
        public IUserRepository Users { get; }
        public ILoginRepository Logins { get; }
        public ICredentialVerificationRepository CredentialVerifications { get; }

        public CreateUserUnitOfWork(
            TesteDbContext context, 
            IUserRepository users, 
            ILoginRepository logins, 
            ICredentialVerificationRepository credentialVerifications
        ) : base(context)
        {
            Users = users;
            Logins = logins;
            CredentialVerifications = credentialVerifications;
        }
    }
}
