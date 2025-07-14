namespace Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent
{
    public class StockReservedEvent : OrderProcessingEvent
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }

        public StockReservedEvent(int orderId, int productId, int quantity)
            : base(orderId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
