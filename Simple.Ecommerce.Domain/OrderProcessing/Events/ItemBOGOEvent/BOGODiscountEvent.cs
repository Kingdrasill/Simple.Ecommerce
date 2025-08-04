using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent
{
    public class BOGODiscountEvent : BaseOrderProcessingEvent
    {
        public int OrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public int? CouponId { get; private set; }
        public string? CouponCode { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public BOGODiscountEvent(int orderId, int orderItemId, int productId, string productName, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal)
            : base(orderId)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductName = productName;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            CouponId = couponId;
            CouponCode = couponCode;
            CurrentTotal = currentTotal;
        }
    }
}
