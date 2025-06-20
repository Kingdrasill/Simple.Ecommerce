using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class CacheFrequencyDeletedEventHandler : IDeleteEventHandler<CacheFrequencyDeletedEvent>
    {
        public Task Handle(CacheFrequencyDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
