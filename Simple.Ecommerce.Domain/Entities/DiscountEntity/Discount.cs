using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.DiscountEntity
{
    public class Discount : BaseEntity
    {
        public string Name { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public DiscountScope DiscountScope { get; private set; }
        public DiscountValueType? DiscountValueType { get; private set; }
        public decimal? Value { get; private set; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }
        public bool IsActive { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<DiscountTier> DiscountTiers { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<DiscountBundleItem> DiscountBundleItems { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Coupon> Coupons { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<ProductDiscount> ProductDiscounts { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Order> Orders { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }

        public Discount() 
        {
            DiscountTiers = new HashSet<DiscountTier>();
            DiscountBundleItems = new HashSet<DiscountBundleItem>();
            Coupons = new HashSet<Coupon>();
            ProductDiscounts = new HashSet<ProductDiscount>();
            Orders = new HashSet<Order>();
            OrderItems = new HashSet<OrderItem>();
        }

        private Discount(int id, string name, DiscountType discountType, DiscountScope discountScope, DiscountValueType? discountValueType, decimal? value, DateTime? validFrom, DateTime? validTo, bool isActive)
        {
            Id = id;
            Name = name;
            DiscountType = discountType;
            DiscountScope = discountScope;
            DiscountValueType = discountValueType;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
            IsActive = isActive;

            DiscountTiers = new HashSet<DiscountTier>();
            DiscountBundleItems = new HashSet<DiscountBundleItem>();
            Coupons = new HashSet<Coupon>();
            ProductDiscounts = new HashSet<ProductDiscount>();
            Orders = new HashSet<Order>();
            OrderItems = new HashSet<OrderItem>();
        }

        public Result<Discount> Create(int id, string name, DiscountType discountType, DiscountScope discountScope, DiscountValueType? discountValueType, decimal? value, DateTime? validFrom, DateTime? validTo, bool isActive)
        {
            return new DiscountValidator().Validate(new Discount(id, name, discountType, discountScope, discountValueType, value, validFrom, validTo, isActive));
        }

        public void SetActivity(bool active) => IsActive = active;

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new DiscountDeletedEvent(Id));
        }
    }
}
