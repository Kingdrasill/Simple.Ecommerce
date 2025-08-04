using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent
{
    public class BOGODiscountRevertEvent : BOGODiscountEvent
    {
        public decimal AmountReverted { get; private set; }

        public BOGODiscountRevertEvent(int orderId, int originalOrderItemId, int productId, string productName, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal, decimal amountReverted)
            : base(orderId, originalOrderItemId, productId, productName, discountId, discountName, discountType, couponId, couponCode, currentTotal)
        {
            AmountReverted = amountReverted;
        }
    }
}
