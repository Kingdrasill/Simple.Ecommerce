using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent
{
    public class BOGOItemDiscountRevertEvent : OrderProcessingEvent
    {
        public int OriginalOrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal AmountReverted { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public BOGOItemDiscountRevertEvent(int orderId, int originalOrderItemId, int productId, string productName, int discountId, string discountName, DiscountType discountType, decimal amountReverted, decimal currentTotal)
            : base(orderId)
        {
            OriginalOrderItemId = originalOrderItemId;
            ProductId = productId;
            ProductName = productName;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            AmountReverted = amountReverted;
            CurrentTotal = currentTotal;
        }
    }
}
