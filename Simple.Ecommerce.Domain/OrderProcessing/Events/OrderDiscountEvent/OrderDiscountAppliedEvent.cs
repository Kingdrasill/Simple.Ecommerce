using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent
{
    public class OrderDiscountAppliedEvent : OrderDiscountEvent
    {
        public decimal AmountDiscounted { get; private set; }

        public OrderDiscountAppliedEvent(int orderId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal, decimal amountDiscounted) 
            : base(orderId, discountId, discountName, discountType, couponId, couponCode, currentTotal)
        {
            AmountDiscounted = amountDiscounted;
        }
    }
}
