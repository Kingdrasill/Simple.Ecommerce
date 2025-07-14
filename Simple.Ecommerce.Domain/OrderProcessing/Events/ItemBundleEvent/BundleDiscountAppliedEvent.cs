using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleDiscountAppliedEvent : OrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public List<BundleItemEntry> BundleItems { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public BundleDiscountAppliedEvent(int orderId, int discountId, string discountName, DiscountType discountType, List<BundleItemEntry> bundleItems, decimal amountDiscountedTotal, decimal currentTotal)
            : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            BundleItems = bundleItems;
            AmountDiscountedTotal = amountDiscountedTotal;
            CurrentTotal = currentTotal;
        }
    }
}
