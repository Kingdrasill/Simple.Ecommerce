using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent
{
    public class TieredItemDiscountAppliedEvent : OrderProcessingEvent
    {
        public int OrderItemId { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public string TierName { get; private set; }
        public decimal AmountDiscountedPrice { get; private set; }
        public decimal NewItemPrice { get; private set; }
        public decimal AmountDiscountedTotal { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public TieredItemDiscountAppliedEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, string tierName, decimal amountDiscountedPrice, decimal newItemPrice, decimal amountDiscountedTotal, decimal currentTotal)
            : base(orderId)
        {
            OrderItemId = orderItemId;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            TierName = tierName;
            AmountDiscountedPrice = amountDiscountedPrice;
            NewItemPrice = newItemPrice;
            AmountDiscountedTotal = amountDiscountedTotal;
            CurrentTotal = currentTotal;
        }
    }
}
