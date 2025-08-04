namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class DiscountReadDetail
    {
        public int DiscountId { get; set; }
        public string DiscountName { get; set; }
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
    }
}
