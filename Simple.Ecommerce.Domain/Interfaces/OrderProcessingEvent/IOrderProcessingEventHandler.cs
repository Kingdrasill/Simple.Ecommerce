namespace Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent
{
    public interface IOrderProcessingEventHandler<TEvent> where TEvent : IOrderProcessingEvent
    {
        Task Handle(TEvent @event);
    }
}
