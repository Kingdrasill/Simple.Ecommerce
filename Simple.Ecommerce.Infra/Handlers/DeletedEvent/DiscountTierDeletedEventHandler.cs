using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class DiscountTierDeletedEventHandler : IDeleteEventHandler<DiscountTierDeletedEvent>
    {
        public Task Handle(DiscountTierDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
