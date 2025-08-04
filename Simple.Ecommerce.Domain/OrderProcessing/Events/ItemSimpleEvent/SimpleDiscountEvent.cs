using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent
{
    public class SimpleDiscountEvent : BaseOrderProcessingEvent
    {
        public int OrderItemId { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }
        public decimal ItemPrice { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public SimpleDiscountEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal itemPrice, decimal currentTotal)
            : base(orderId)
        {
            OrderItemId = orderItemId;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            CouponId = couponId;
            CouponCode = couponCode;
            ItemPrice = itemPrice;
            CurrentTotal = currentTotal;
        }
    }
}
