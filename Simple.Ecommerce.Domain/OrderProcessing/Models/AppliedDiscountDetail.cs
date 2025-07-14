using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class AppliedDiscountDetail
    {

        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }

        public AppliedDiscountDetail(int discountId, string discountName, DiscountType discountType)
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
        }
    }
}
