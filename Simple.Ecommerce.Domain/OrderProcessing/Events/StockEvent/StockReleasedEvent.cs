namespace Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent
{
    public class StockReleasedEvent : StockEvent
    {
        public StockReleasedEvent(int orderId, int productId, int quantity)
            : base(orderId, productId, quantity) { }
    }
}
