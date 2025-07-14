namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderProcessedEvent : OrderProcessingEvent
    {
        public string Status { get; private set; }
        public decimal FinalTotal { get; private set; }

        public OrderProcessedEvent(int orderId, string status, decimal finalTotal)
            : base(orderId)
        {
            Status = status;
            FinalTotal = finalTotal;
        }
    }
}
