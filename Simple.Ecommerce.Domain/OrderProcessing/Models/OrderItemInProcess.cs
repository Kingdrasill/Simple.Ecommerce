using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class OrderItemInProcess
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public AppliedDiscountDetail? AppliedDiscount { get; private set; }

        public OrderItemInProcess(int id, int productId, string productName, int quantity, decimal unitPrice)
        {
            Id = id;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CurrentPrice = unitPrice;
            DiscountAmount = 0;
            AppliedDiscount = null;
        }

        public OrderItemInProcess(int id, int productId, string productName, int quantity, decimal unitPrice, decimal currentPrice, decimal discountAmount, AppliedDiscountDetail? appliedDiscount)
        {
            Id = id;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CurrentPrice = currentPrice;
            DiscountAmount = discountAmount;
            AppliedDiscount = appliedDiscount;
        }

        public void ApplyDiscount(int discountId, string discountName, DiscountType discountType, decimal discountAmount)
        {
            DiscountAmount = discountAmount;
            CurrentPrice -= discountAmount;
            AppliedDiscount = new AppliedDiscountDetail(discountId, discountName, discountType);
        }

        public void RevertDiscount(decimal amountReverted)
        {
            DiscountAmount -= amountReverted;
            CurrentPrice += amountReverted;
            AppliedDiscount = null;
        }

        public void AddNItems(int n)
        {
            Quantity += n;
        }

        public OrderItemInProcess RemoveNItems(int n)
        {
            Quantity -= n;
            return new OrderItemInProcess(
                Id,
                ProductId,
                ProductName,
                n,
                UnitPrice,
                CurrentPrice,
                DiscountAmount,
                AppliedDiscount
            );
        }

        public decimal GetTotal()
        {
            return CurrentPrice * Quantity;
        }
    }
}
