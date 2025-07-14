using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class CategoryDeletedEvent : IDeleteEvent
    {
        public int CategoryId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public CategoryDeletedEvent(int categoryId) 
        {
            CategoryId = categoryId;
        }
    }
}
