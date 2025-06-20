namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class OrderCanceledEvent
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public DateTime CanceledAt { get; set; }
    }
}
