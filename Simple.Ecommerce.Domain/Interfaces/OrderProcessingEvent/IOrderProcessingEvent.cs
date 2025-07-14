namespace Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent
{
    public interface IOrderProcessingEvent
    {
        Guid EventId { get; }
        int AggregateId { get; }
        DateTime Timestamp { get; }
        string EventType { get; }
    }
}
