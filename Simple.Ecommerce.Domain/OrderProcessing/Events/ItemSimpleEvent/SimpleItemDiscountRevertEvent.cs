using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent
{
    public class SimpleItemDiscountRevertEvent : SimpleDiscountEvent
    {
        public decimal AmountRevertedPrice { get; private set; }
        public decimal AmountRevertedTotal { get; private set; }

        public SimpleItemDiscountRevertEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal itemPrice, decimal currentTotal, decimal amountRevertedPrice, decimal amountRevertedTotal)
            : base(orderId, orderItemId, discountId, discountName, discountType, couponId, couponCode, itemPrice, currentTotal)
        {
            AmountRevertedPrice = amountRevertedPrice;
            AmountRevertedTotal = amountRevertedTotal;
        }
    }
}
