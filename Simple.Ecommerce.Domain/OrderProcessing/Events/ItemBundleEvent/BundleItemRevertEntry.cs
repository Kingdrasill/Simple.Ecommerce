namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleItemRevertEntry : BundleItemEntry
    {
        public decimal AmountRevertedPrice { get; private set; }

        public BundleItemRevertEntry(int orderItemId, int productId, string productName, int quantity, decimal itemPrice, decimal amountRevertedPrice)
            : base(orderItemId, productId, productName, quantity, itemPrice)
        {
            AmountRevertedPrice = amountRevertedPrice;
        }
    }
}
