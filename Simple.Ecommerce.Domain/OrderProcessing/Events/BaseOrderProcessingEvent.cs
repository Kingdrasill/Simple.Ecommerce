using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events
{
    public abstract class BaseOrderProcessingEvent : IOrderProcessingEvent
    {
        public Guid EventId { get; private set; }
        public int AggregateId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string EventType => GetType().Name;

        protected BaseOrderProcessingEvent(int aggregateId)
        {
            EventId = Guid.NewGuid();
            AggregateId = aggregateId;
            Timestamp = DateTime.UtcNow;
        }
    }
}
