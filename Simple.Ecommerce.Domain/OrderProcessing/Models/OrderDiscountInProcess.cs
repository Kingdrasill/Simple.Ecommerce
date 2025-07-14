using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Domain.OrderProcessing.Models
{
    public class OrderDiscountInProcess
    {
        public int Id { get; private set; }
        public int OwnerId { get; private set; }
        public string Name { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public DiscountScope DiscountScope { get; private set; }
        public DiscountValueType? DiscountValueType { get; private set; }
        public decimal? Value { get; private set; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }
        public bool IsActive { get; private set; }

        public OrderDiscountInProcess(int id, int ownerId, string name, DiscountType discountType, DiscountScope discountScope, DiscountValueType? discountValueType, decimal? value, DateTime? validFrom, DateTime? validTo, bool isActive)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            DiscountType = discountType;
            DiscountScope = discountScope;
            DiscountValueType = discountValueType;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
            IsActive = isActive;
        }
    }
}
