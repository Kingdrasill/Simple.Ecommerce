namespace Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent
{
    public class StockReservedEvent : StockEvent
    {
        public StockReservedEvent(int orderId, int productId, int quantity)
            : base(orderId, productId, quantity) { }
    }
}
