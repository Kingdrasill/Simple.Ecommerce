using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent
{
    public class SimpleItemDiscountAppliedEvent : SimpleDiscountEvent
    {
        public decimal AmountDiscountedPrice { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }

        public SimpleItemDiscountAppliedEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal itemPrice, decimal currentTotal, decimal amountDiscountedPrice, decimal amountDiscountedTotal)
            : base(orderId, orderItemId, discountId, discountName, discountType, couponId, couponCode, itemPrice, currentTotal)
        {
            AmountDiscountedPrice = amountDiscountedPrice;
            AmountDiscountedTotal = amountDiscountedTotal;
        }
    }
}
