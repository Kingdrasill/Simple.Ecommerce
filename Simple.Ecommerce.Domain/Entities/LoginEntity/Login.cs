using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.LoginEntity
{
    public class Login : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Credential { get; private set; }
        public string Password { get; private set; }
        public CredentialType Type { get; private set; }
        public bool IsVerified { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<CredentialVerification> CredentialVerifications { get; private set; }

        public Login()
        {
            CredentialVerifications = new HashSet<CredentialVerification>();
        }

        private Login(int id, int userId, string credential, string password, CredentialType type)
        {
            Id = id;
            UserId = userId;
            Credential = credential;
            Password = password;
            Type = type;
            IsVerified = false;

            CredentialVerifications = new HashSet<CredentialVerification>();
        }

        private Login(int id, User user, string credential, string password, CredentialType type)
        {
            Id = id;
            User = user;
            Credential = credential;
            Password = password;
            Type = type;
            IsVerified = false;

            CredentialVerifications = new HashSet<CredentialVerification>();
        }

        public Result<Login> Create(int id, int userId, string credential, string password, CredentialType type)
        {
            return new LoginValidator().Validate(new Login(id, userId, credential, password, type));
        }

        public Result<Login> Create(int id, User user, string credential, string password, CredentialType type)
        {
            return new LoginValidator().Validate(new Login(id, user, credential, password, type));
        }

        public void SetVerified() => IsVerified = true;

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new LoginDeletedEvent(Id));
        }
    }
}
