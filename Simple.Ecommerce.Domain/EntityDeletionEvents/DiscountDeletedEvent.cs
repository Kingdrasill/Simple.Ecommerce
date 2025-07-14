using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class DiscountDeletedEvent : IDeleteEvent
    {
        public int DiscountId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public DiscountDeletedEvent(int discountId) 
        {
            DiscountId = discountId;
        }
    }
}
