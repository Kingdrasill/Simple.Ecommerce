using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
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
