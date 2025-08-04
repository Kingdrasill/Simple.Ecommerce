namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleItemEntry
    {
        public int OrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal ItemPrice { get; private set; }

        public BundleItemEntry(int orderItemId, int productId, string productName, int quantity, decimal itemPrice)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            ItemPrice = itemPrice;
        }
    }
}
