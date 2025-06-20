namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class OrderDeliveredEvent
    {
        public string OrderId { get; set; }
        public DateTime DeliveredAt { get; set; }
    }
}
