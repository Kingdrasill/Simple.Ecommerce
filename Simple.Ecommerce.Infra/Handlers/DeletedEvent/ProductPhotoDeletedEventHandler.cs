using Simple.Ecommerce.Domain.Events.DeletedEvent;
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
