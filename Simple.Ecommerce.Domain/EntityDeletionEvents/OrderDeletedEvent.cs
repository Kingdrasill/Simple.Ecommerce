using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class OrderDeletedEvent : IDeleteEvent
    {
        public int OrderId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public OrderDeletedEvent(int orderId) 
        {
            OrderId = orderId;
        }
    }
}
