using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class CouponDeletedEvent : IDeleteEvent
    {
        public int CouponId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public CouponDeletedEvent(int couponId)
        {
            CouponId = couponId;
        }
    }
}
