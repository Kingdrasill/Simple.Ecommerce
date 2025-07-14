using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class ReviewDeletedEvent : IDeleteEvent
    {
        public int ReviewId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public ReviewDeletedEvent(int reviewId)
        {
            ReviewId = reviewId;
        }
    }
}
