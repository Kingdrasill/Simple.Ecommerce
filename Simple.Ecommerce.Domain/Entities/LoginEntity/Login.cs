using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Enums.Crendetial;
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

        private Login(int id, int? userId, User? user, string credential, string password, CredentialType type, bool? isVerified)
        {
            Id = id;
            if (userId.HasValue)
                UserId = userId.Value;
            if (user is not null)
                User = user;
            Credential = credential;
            Password = password;
            Type = type;
            if (isVerified.HasValue)
                IsVerified = isVerified.Value;
        }

        public Result<Login> Create(int id, int? userId, User? user, string credential, string password, CredentialType type, bool? isVerified)
        {
            return new LoginValidator().Validate(new Login(id, userId, user, credential, password, type, isVerified));
        }

        public Result<Login> Validate()
        {
            return new LoginValidator().Validate(this);
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
