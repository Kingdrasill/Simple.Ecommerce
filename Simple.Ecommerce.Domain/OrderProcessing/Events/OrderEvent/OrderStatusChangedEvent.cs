namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderStatusChangedEvent : OrderProcessingEvent
    {
        public string NewStatus { get; private set; }

        public OrderStatusChangedEvent(int orderId, string newStatus)
            : base(orderId)
        {
            NewStatus = newStatus;
        }
    }
}
