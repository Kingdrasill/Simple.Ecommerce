using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent
{
    public class OrderDiscountEvent : BaseOrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public OrderDiscountEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal)
            : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            CouponId = couponId;
            CouponCode = couponCode;
            CurrentTotal = currentTotal;
        }
    }
}
