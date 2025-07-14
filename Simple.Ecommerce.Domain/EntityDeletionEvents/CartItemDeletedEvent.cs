using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class CartItemDeletedEvent : IDeleteEvent
    {
        public int CartItemId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public CartItemDeletedEvent(int cartItemId) 
        {
            CartItemId = cartItemId;
        }
    }
}
