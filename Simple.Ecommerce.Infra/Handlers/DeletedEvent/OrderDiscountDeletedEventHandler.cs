using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class OrderDiscountDeletedEventHandler : IDeleteEventHandler<OrderDiscountDeletedEvent>
    {
        public Task Handle(OrderDiscountDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
