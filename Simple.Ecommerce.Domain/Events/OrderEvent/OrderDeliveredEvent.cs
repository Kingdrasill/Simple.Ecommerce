namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class OrderDeliveredEvent
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime DeliveredAt { get; set; }
    }
}
