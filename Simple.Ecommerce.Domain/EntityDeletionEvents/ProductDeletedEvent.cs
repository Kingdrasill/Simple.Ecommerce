using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class ProductDeletedEvent : IDeleteEvent
    {
        public int ProductId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public ProductDeletedEvent(int productId)
        {
            ProductId = productId;
        }
    }
}
