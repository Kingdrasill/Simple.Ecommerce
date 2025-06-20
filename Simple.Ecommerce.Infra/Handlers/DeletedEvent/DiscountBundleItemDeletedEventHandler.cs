using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class DiscountBundleItemDeletedEventHandler : IDeleteEventHandler<DiscountBundleItemDeletedEvent>
    {
        public Task Handle(DiscountBundleItemDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
