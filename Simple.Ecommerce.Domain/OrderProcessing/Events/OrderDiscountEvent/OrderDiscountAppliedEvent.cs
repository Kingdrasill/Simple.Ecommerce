using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent
{
    public class OrderDiscountAppliedEvent : OrderProcessingEvent
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal AmountDiscounted { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public OrderDiscountAppliedEvent(int orderId, int discountId, string discountName, DiscountType discountType, decimal amountDiscounted, decimal currentTotal) 
            : base(orderId)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            AmountDiscounted = amountDiscounted;
            CurrentTotal = currentTotal;
        }
    }
}
