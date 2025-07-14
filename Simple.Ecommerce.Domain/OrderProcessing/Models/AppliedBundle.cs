using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class AppliedBundle
    {
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public List<AppliedDiscountItem> Items { get; private set; }

        public AppliedBundle(int discountId, string discountName, DiscountType discountType, List<AppliedDiscountItem> items) 
        {
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            Items = items;
        }
    }
}
