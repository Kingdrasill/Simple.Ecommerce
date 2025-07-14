namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleItemEntry
    {
        public int OriginalOrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal AmountDiscountedPrice { get; private set; }
        public decimal NewItemPrice { get; private set; }

        public BundleItemEntry(int originalOrderItemId, int productId, string productName, int quantity, decimal amountDiscountedPrice, decimal newItemPrice)
        {
            OriginalOrderItemId = originalOrderItemId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            AmountDiscountedPrice = amountDiscountedPrice;
            NewItemPrice = newItemPrice;
        }
    }
}
