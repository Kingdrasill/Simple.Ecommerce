using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent
{
    public class TieredItemDiscountAppliedEvent : TieredDiscountEvent
    {
        public decimal AmountDiscountedPrice { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }

        public TieredItemDiscountAppliedEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int tierId, string tierName, int? couponId, string? couponCode, decimal itemPrice, decimal currentTotal, decimal amountDiscountedPrice, decimal amountDiscountedTotal)
            : base(orderId, orderItemId, discountId, discountName, discountType, tierId, tierName, couponId, couponCode, itemPrice, currentTotal)
        {
            AmountDiscountedPrice = amountDiscountedPrice;
            AmountDiscountedTotal = amountDiscountedTotal;
        }
    }
}
