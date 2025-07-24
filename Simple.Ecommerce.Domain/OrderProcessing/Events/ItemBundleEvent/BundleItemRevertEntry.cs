namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleItemRevertEntry
    {
        public int OriginalOrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal AmountRevertedPrice { get; private set; }
        public decimal ItemPrice { get; private set; }

        public BundleItemRevertEntry(int originalOrderItemId, int productId, string productName, int quantity, decimal amountRevertedPrice, decimal itemPrice)
        {
            OriginalOrderItemId = originalOrderItemId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            AmountRevertedPrice = amountRevertedPrice;
            ItemPrice = itemPrice;
        }
    }
}
