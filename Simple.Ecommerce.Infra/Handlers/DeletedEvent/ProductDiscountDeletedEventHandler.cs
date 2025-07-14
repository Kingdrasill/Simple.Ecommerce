using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class ProductDiscountDeletedEventHandler : IDeleteEventHandler<ProductDiscountDeletedEvent>
    {
        public Task Handle(ProductDiscountDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
