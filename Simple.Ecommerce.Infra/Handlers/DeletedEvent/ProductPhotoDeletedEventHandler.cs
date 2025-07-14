using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class ProductPhotoDeletedEventHandler : IDeleteEventHandler<ProductPhotoDeletedEvent>
    {
        public Task Handle(ProductPhotoDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
