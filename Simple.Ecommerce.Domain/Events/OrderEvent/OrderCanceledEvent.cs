namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class OrderCanceledEvent
    {
        public string OrderId { get; set; }
        public string Reason { get; set; }
        public DateTime CanceledAt { get; set; }
    }
}
