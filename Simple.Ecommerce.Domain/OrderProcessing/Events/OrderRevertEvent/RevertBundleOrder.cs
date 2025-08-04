namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent
{
    public class RevertBundleOrder
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponName { get; private set; }
        public Guid BundleId { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }
        public List<RevertBundleItemOrderEntry> BundleItems { get; private set; }

        public RevertBundleOrder(int discountId, string discountName, int? couponId, string? couponName, Guid bundleId, decimal amountDiscountedTotal, List<RevertBundleItemOrderEntry> bundleItems)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            CouponId = couponId;
            CouponName = couponName;
            BundleId = bundleId;
            AmountDiscountedTotal = amountDiscountedTotal;
            BundleItems = bundleItems;
        }
    }
}
