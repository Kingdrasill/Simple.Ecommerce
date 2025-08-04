using Simple.Ecommerce.Domain.Enums.OrderLock;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent
{
    public class OrderRevertedEvent : BaseOrderProcessingEvent
    {
        public string Status { get; private set; }
        public OrderLock OrderLock { get; private set; }
        public decimal OriginalTotal { get; private set; }

        public OrderRevertedEvent(int orderId, string status, OrderLock orderLock, decimal originalTotal) 
            : base(orderId) 
        { 
            Status = status;
            OrderLock = orderLock;
            OriginalTotal = originalTotal;
        }
    }
}
