using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleDiscountRevertEvent : BundleDiscountEvent
    {
        public List<BundleItemRevertEntry> RevertedBundleItems { get; private set; }
        public decimal AmountRevertedTotal { get; private set; }

        public BundleDiscountRevertEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, Guid bundleId, decimal currentTotal, List<BundleItemRevertEntry> revertedBundleItems, decimal amountRevertedTotal)
            : base(orderId, discountId, discountName, discountType, couponId, couponCode, bundleId, currentTotal)
        {
            RevertedBundleItems = revertedBundleItems;
            AmountRevertedTotal = amountRevertedTotal;
        }
    }
}
