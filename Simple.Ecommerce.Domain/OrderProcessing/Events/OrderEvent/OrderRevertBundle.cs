namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderRevertBundle
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public Guid BundleId { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }
        public List<OrderRevertBundleItemEntry> BundleItems { get; private set; }

        public OrderRevertBundle(int discountId, string discountName, Guid bundleId, decimal amountDiscountedTotal, List<OrderRevertBundleItemEntry> bundleItems)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            BundleId = bundleId;
            AmountDiscountedTotal = amountDiscountedTotal;
            BundleItems = bundleItems;
        }
    }
}
