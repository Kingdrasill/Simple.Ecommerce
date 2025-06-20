using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class DiscountTierDeletedEvent : IDeleteEvent
    {
        public int DiscountTierId { get; } 
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public DiscountTierDeletedEvent(int discountTierId)
        {
            DiscountTierId = discountTierId;
        }
    }
}
