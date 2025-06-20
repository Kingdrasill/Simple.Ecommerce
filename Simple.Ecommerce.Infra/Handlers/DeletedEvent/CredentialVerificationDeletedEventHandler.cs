using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class CredentialVerificationDeletedEventHandler : IDeleteEventHandler<CredentialVerificationDeletedEvent>
    {
        public Task Handle(CredentialVerificationDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
