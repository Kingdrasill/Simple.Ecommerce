using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class UserPaymentDeletedEvent : IDeleteEvent
    {
        public int UserPaymentId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public UserPaymentDeletedEvent(int userPaymentId)
        {
            UserPaymentId = userPaymentId;
        }
    }
}
