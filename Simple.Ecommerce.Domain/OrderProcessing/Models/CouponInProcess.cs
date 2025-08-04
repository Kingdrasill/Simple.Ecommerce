namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class CouponInProcess
    {
        public int Id { get; private set; }
        public int DiscountId { get; private set; }
        public string Code { get; private set; }
        public DateTime ExpirationAt { get; private set; }
        public bool IsUsed { get; private set; }

        public CouponInProcess(int id, int discountId, string code, DateTime expirationAt, bool isUsed)
        {
            Id = id;
            DiscountId = discountId;
            Code = code;
            ExpirationAt = expirationAt;
            IsUsed = isUsed;
        }
    }
}
