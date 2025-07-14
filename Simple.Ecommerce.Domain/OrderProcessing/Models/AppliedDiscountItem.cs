namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class AppliedDiscountItem
    {
        public int OriginalOrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public decimal AmountDiscountedPrice { get; private set; }
        public AppliedDiscountDetail AppliedDiscount { get; private set; }

        public AppliedDiscountItem(int originalOrderItemId, int productId, string productName, int quantity, decimal currentPrice, decimal amountDiscountedPrice, AppliedDiscountDetail appliedDiscount)
        {
            OriginalOrderItemId = originalOrderItemId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            CurrentPrice = currentPrice;
            AmountDiscountedPrice = amountDiscountedPrice;
            AppliedDiscount = appliedDiscount;
        }
    }
}
