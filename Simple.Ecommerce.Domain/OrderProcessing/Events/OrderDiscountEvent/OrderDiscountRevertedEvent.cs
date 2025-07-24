using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent
{
    public class OrderDiscountRevertedEvent : OrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal AmountReverted { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public OrderDiscountRevertedEvent(int orderId, int discountId, string discountName, DiscountType discountType, decimal amountReverted, decimal currentTotal)
                : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            AmountReverted = amountReverted;
            CurrentTotal = currentTotal;
        }
    }
}
