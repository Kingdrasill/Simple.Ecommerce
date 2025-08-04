using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent
{
    public class OrderDiscountRevertedEvent : OrderDiscountEvent
    {
        public decimal AmountReverted { get; private set; }

        public OrderDiscountRevertedEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal, decimal amountReverted)
            : base(orderId, discountId, discountName, discountType, couponId, couponCode, currentTotal)
        {
            AmountReverted = amountReverted;
        }
    }
}
