using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class ProductCategoryDeletedEventHandler : IDeleteEventHandler<ProductCategoryDeletedEvent>
    {
        public Task Handle(ProductCategoryDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
