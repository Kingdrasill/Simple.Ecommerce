using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent
{
    public class TieredItemDiscountRevertEvent : TieredDiscountEvent
    {
        public decimal AmountRevertedPrice { get; private set; }
        public decimal AmountRevertedTotal { get; private set; }

        public TieredItemDiscountRevertEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int tierId, string tierName, int? couponId, string? couponCode, decimal amountRevertedPrice, decimal itemPrice, decimal amountRevertedTotal, decimal currentTotal)
            : base(orderId, orderItemId, discountId, discountName, discountType, tierId, tierName, couponId, couponCode, itemPrice, currentTotal)
        {
            AmountRevertedPrice = amountRevertedPrice;
            AmountRevertedTotal = amountRevertedTotal;
        }
    }
}
