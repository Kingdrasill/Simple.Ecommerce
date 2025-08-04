namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent
{
    public class RevertDiscount
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }

        public RevertDiscount(int discountId, string discountName, int? couponId, string? couponCode)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            CouponId = couponId;
            CouponCode = couponCode;
        }
    }
}
