using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class UserPaymentDeletedEventHandler : IDeleteEventHandler<UserPaymentDeletedEvent>
    {
        public Task Handle(UserPaymentDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
