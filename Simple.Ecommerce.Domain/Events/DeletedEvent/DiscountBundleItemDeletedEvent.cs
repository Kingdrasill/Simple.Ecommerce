using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class DiscountBundleItemDeletedEvent : IDeleteEvent
    {
        public int DiscountBundleItemId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public DiscountBundleItemDeletedEvent(int discountBundleItemId)
        {
            DiscountBundleItemId = discountBundleItemId;
        }
    }
}
