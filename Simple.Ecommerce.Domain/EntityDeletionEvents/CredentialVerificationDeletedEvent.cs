using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class CredentialVerificationDeletedEvent : IDeleteEvent
    {
        public int CredentialVerificationId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public CredentialVerificationDeletedEvent(int credentialVerificationId)
        {
            CredentialVerificationId = credentialVerificationId;
        }

    }
}
