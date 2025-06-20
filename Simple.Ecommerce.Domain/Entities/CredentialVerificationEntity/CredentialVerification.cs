using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity
{
    public class CredentialVerification : BaseEntity
    {
        public int LoginId { get; private set; }
        public Login Login { get; private set; } = null!;
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }

        public CredentialVerification() { }

        private CredentialVerification(int id, int loginId, string token, DateTime expiresAt)
        {
            Id = id;
            LoginId = loginId;
            Token = token;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public Result<CredentialVerification> Create(int id, int loginId, string token, DateTime expiresAt)
        {
            return new CredentialVerificationValidator().Validate(new CredentialVerification(id, loginId, token, expiresAt));
        }

        public void MarkAsUsed() => IsUsed = true;

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
