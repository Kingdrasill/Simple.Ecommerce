using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class AppliedBundle
    {
        public Guid Id { get; private set; }
        public int DiscountId { get; private set; }
        public string DiscountName { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public List<AppliedDiscountItem> Items { get; private set; }

        public AppliedBundle(int discountId, string discountName, DiscountType discountType, List<AppliedDiscountItem> items) 
        {
            Id = Guid.NewGuid();
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            Items = items;
        }

        public AppliedBundle(Guid bundleId, int discountId, string discountName, DiscountType discountType, List<AppliedDiscountItem> items)
        {
            Id = bundleId;
            DiscountId = discountId;
            DiscountName = discountName;
            DiscountType = discountType;
            Items = items;
        }
    }
}
