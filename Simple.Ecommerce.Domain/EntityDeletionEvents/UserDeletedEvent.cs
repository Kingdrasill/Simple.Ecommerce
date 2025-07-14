using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class UserDeletedEvent : IDeleteEvent
    {
        public int UserId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public UserDeletedEvent(int userId) 
        {
            UserId = userId;
        }
    }
}
