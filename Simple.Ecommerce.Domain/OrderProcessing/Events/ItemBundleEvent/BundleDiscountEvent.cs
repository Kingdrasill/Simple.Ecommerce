using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent
{
    public class BundleDiscountEvent : BaseOrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }
        public Guid BundleId { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public BundleDiscountEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, Guid bundleId, decimal currentTotal)
            : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            CouponId = couponId;
            CouponCode = couponCode;
            BundleId = bundleId;
            CurrentTotal = currentTotal;
        }
    }
}
