using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class LoginDeletedEvent : IDeleteEvent
    {
        public int LoginId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public LoginDeletedEvent(int loginId)
        {
            LoginId = loginId;
        }
    }
}
