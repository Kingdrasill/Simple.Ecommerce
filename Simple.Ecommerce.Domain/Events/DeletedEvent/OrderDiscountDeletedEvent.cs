using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class OrderDiscountDeletedEvent : IDeleteEvent
    {
        public int OrderDiscountId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public OrderDiscountDeletedEvent(int orderDiscountId) 
        { 
            OrderDiscountId = orderDiscountId;
        }
    }
}
