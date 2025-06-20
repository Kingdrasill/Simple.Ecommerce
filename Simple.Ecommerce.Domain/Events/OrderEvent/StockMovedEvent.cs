namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class StockMovedEvent
    {
        public int ProductId { get; set; }
        public int QuantityMoved { get; set; }
        public string Reason { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}
