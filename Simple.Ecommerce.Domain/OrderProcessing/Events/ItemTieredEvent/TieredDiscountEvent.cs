using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent
{
    public class TieredDiscountEvent : BaseOrderProcessingEvent
    {
        public int OrderItemId { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public int TierId { get; private set; }
        public string TierName { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }
        public decimal ItemPrice { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public TieredDiscountEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int tierId, string tierName, int? couponId, string? couponCode, decimal itemPrice, decimal currentTotal)
            : base(orderId)
        {
            OrderItemId = orderItemId;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            TierId = tierId;
            TierName = tierName;
            CouponId = couponId;
            CouponCode = couponCode;
            ItemPrice = itemPrice;
            CurrentTotal = currentTotal;
        }
    }
}
