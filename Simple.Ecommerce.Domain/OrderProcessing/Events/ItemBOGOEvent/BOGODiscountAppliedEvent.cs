using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent
{
    public class BOGODiscountAppliedEvent : BOGODiscountEvent
    {
        public decimal AmountDiscounted { get; private set; }

        public BOGODiscountAppliedEvent(int orderId, int originalOrderItemId, int productId, string productName, int discountId, string discountName, DiscountType discountType, int? couponId, string? couponCode, decimal currentTotal, decimal amountDiscounted)
            : base(orderId, originalOrderItemId, productId, productName, discountId, discountName, discountType, couponId, couponCode, currentTotal)
        {
            AmountDiscounted = amountDiscounted;
        }
    }
}
