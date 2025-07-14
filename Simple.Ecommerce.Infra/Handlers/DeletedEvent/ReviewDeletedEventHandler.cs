using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class ReviewDeletedEventHandler : IDeleteEventHandler<ReviewDeletedEvent>
    {
        public Task Handle(ReviewDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
