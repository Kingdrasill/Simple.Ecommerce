using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class ProductPhotoDeletedEvent : IDeleteEvent
    {
        public int ProductPhotoId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public ProductPhotoDeletedEvent(int productPhotoId)
        {
            ProductPhotoId = productPhotoId;
        }
    }
}
