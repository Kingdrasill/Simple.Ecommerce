using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class UserCardDeletedEvent : IDeleteEvent
    {
        public int UserCardId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public UserCardDeletedEvent(int userCardId)
        {
            UserCardId = userCardId;
        }
    }
}
