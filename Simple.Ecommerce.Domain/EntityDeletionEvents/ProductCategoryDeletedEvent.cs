using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class ProductCategoryDeletedEvent : IDeleteEvent
    {
        public int ProductCategoryId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public ProductCategoryDeletedEvent(int productCategoryId)
        {
            ProductCategoryId = productCategoryId;
        }
    }
}
