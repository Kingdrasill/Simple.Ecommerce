using Simple.Ecommerce.Domain.Events.DeletedEvent;
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
