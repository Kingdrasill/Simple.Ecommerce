namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderItemEntry
    {
        public int OrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        public OrderItemEntry(int orderItemId, int productId, string productName, decimal price, int quantity)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }
    }
}
