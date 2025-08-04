namespace Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent
{
    public class StockEvent : BaseOrderProcessingEvent
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }

        public StockEvent(int orderId, int productId, int quantity)
            : base(orderId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
