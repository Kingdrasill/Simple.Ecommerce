using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class CouponDeletedEventHandler : IDeleteEventHandler<CouponDeletedEvent>
    {
        public Task Handle(CouponDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
