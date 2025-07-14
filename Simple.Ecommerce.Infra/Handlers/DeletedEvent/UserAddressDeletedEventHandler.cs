using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class UserAddressDeletedEventHandler : IDeleteEventHandler<UserAddressDeletedEvent>
    {
        public Task Handle(UserAddressDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
