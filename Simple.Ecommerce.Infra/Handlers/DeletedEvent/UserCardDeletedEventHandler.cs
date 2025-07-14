using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class UserCardDeletedEventHandler : IDeleteEventHandler<UserCardDeletedEvent>
    {
        public Task Handle(UserCardDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
