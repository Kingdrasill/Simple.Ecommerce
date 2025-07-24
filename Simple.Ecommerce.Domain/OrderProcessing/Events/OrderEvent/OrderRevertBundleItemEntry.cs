namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderRevertBundleItemEntry
    {
        public int OrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal AmountDiscountedPrice { get; private set; }

        public OrderRevertBundleItemEntry(int orderItemId, int productId, string productName, int quantity, decimal unitPrice, decimal amountDiscountedPrice)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            AmountDiscountedPrice = amountDiscountedPrice;
        }
    }
}
