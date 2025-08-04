using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleDiscountAppliedEvent : BundleDiscountEvent
    {
        public List<BundleItemAppliedEntry> BundleItems { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }

        public BundleDiscountAppliedEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, Guid bundleId, decimal currentTotal, List<BundleItemAppliedEntry> bundleItems, decimal amountDiscountedTotal)
            : base(orderId, discountId, discountName, discountType, couponId, couponCode, bundleId, currentTotal)
        {
            BundleItems = bundleItems;
            AmountDiscountedTotal = amountDiscountedTotal;
        }
    }
}
