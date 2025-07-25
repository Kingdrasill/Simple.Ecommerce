using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;

namespace Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity
{
    public class CredentialVerification : BaseEntity
    {
        public int LoginId { get; private set; }
        public Login Login { get; private set; } = null!;
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        public CredentialVerification() { }

        private CredentialVerification(int id, int? loginId, Login? login, string token, DateTime expiresAt, bool? isUsed, DateTime? usedAt)
        {
            Id = id;
            if (loginId.HasValue)
                LoginId = loginId.Value;
            if (login is not null)
                Login = login;
            Token = token;
            ExpiresAt = expiresAt;
            if (isUsed.HasValue)
                IsUsed = isUsed.Value;
            else
                IsUsed = false;
            if (usedAt.HasValue)
                UsedAt = usedAt.Value;
            else
                UsedAt = null;
        }

        public Result<CredentialVerification> Create(int id, int? loginId, Login? login, string token, DateTime expiresAt, bool? isUsed, DateTime? usedAt)
        {
            return new CredentialVerificationValidator().Validate(new CredentialVerification(id, loginId, login, token, expiresAt, isUsed, usedAt));
        }

        public Result<CredentialVerification> Validate()
        {
            return new CredentialVerificationValidator().Validate(this);
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new CredentialVerificationDeletedEvent(Id));
        }
    }
}
