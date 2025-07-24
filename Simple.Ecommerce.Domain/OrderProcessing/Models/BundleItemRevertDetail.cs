namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class BundleItemRevertDetail
    {
        public int OriginalOrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal AmountRevertedPrice { get; private set; }

        public BundleItemRevertDetail(int originalOrderItemId, int productId, int quantity, decimal amountRevertedPrice)
        {
            OriginalOrderItemId = originalOrderItemId;
            ProductId = productId;
            Quantity = quantity;
            AmountRevertedPrice = amountRevertedPrice;
        }
    }
}
