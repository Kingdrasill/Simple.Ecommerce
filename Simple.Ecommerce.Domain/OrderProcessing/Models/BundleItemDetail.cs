namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class BundleItemDetail
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal AmountDiscountedPrice { get; private set; }

        public BundleItemDetail(int productId, int quantity, decimal amountDiscountedPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            AmountDiscountedPrice = amountDiscountedPrice;
        }
    }
}
