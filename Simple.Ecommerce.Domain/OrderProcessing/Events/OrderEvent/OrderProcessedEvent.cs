using Simple.Ecommerce.Domain.Enums.OrderLock;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderProcessedEvent : OrderProcessingEvent
    {
        public string Status { get; private set; }
        public decimal FinalTotal { get; private set; }
        public OrderLock OrderLock { get; private set; }

        public OrderProcessedEvent(int orderId, string status, decimal finalTotal, OrderLock orderLock)
            : base(orderId)
        {
            Status = status;
            FinalTotal = finalTotal;
            OrderLock = orderLock;
        }
    }
}
