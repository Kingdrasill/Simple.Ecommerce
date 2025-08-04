namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleItemAppliedEntry : BundleItemEntry
    {
        public decimal AmountDiscountedPrice { get; private set; }

        public BundleItemAppliedEntry(int orderItemId, int productId, string productName, int quantity, decimal itemPrice, decimal amountDiscountedPrice)
            : base(orderItemId, productId, productName, quantity, itemPrice)
        {
            AmountDiscountedPrice = amountDiscountedPrice;
        }
    }
}
