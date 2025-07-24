using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleDiscountRevertEvent : OrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public Guid BundleId { get; private set; }
        public List<BundleItemRevertEntry> BundleItems { get; private set; }
        public decimal AmountRevertedTotal { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public BundleDiscountRevertEvent(int orderId, int discountId, string discountName, DiscountType discountType, Guid bundleId, List<BundleItemRevertEntry> bundleItems, decimal amountRevertedTotal, decimal currentTotal)
            : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            BundleId = bundleId;
            BundleItems = bundleItems;
            AmountRevertedTotal = amountRevertedTotal;
            CurrentTotal = currentTotal;
        }
    }
}
