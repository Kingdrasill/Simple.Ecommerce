namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class RevertBundleItemDetail
    {
        public int OrderItemId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal AmountRevertedPrice { get; private set; }

        public RevertBundleItemDetail(int orderItemId, int productId, int quantity, decimal amountRevertedPrice)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            Quantity = quantity;
            AmountRevertedPrice = amountRevertedPrice;
        }
    }
}
