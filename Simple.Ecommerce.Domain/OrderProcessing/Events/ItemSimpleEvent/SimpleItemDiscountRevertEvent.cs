using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent
{
    public class SimpleItemDiscountRevertEvent : OrderProcessingEvent
    {
        public int OrderItemId { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal AmountRevertedPrice { get; private set; }
        public decimal ItemPrice { get; private set; }
        public decimal AmountRevertedTotal { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public SimpleItemDiscountRevertEvent(int orderId, int orderItemId, int discountId, string discountName, DiscountType discountType, decimal amountRevertedPrice, decimal itemPrice, decimal amountRevertedTotal, decimal currentTotal)
            : base(orderId)
        {
            OrderItemId = orderItemId;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            AmountRevertedPrice = amountRevertedPrice;
            ItemPrice = itemPrice;
            AmountRevertedTotal = amountRevertedTotal;
            CurrentTotal = currentTotal;
        }
    }
}
