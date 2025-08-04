using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.CouponEntity
{
    public class Coupon : BaseEntity
    {
        public string Code { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpirationAt { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;
        [IgnoreDataMember, NotMapped]
        public ICollection<Order> Orders { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }

        public Coupon()
        {
            Orders = new HashSet<Order>();
            OrderItems = new HashSet<OrderItem>();
        }

        private Coupon(int id, string code, DateTime expirationAt, int discountId, bool isUsed = false, DateTime? usedAt = null, DateTime? createdAt = null)
        {
            Id = id; 
            Code = code;
            IsUsed = isUsed;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            ExpirationAt = expirationAt;
            UsedAt = usedAt;
            DiscountId = discountId;

            Orders = new HashSet<Order>();
            OrderItems = new HashSet<OrderItem>();
        }

        public Result<Coupon> Create(int id, string code, DateTime expirationAt, int discountId, bool isUsed = false, DateTime? usedAt = null, DateTime? createdAt = null)
        {
            return new CouponValidator().Validate(new Coupon(id, code, expirationAt, discountId, isUsed, usedAt, createdAt));
        }

        public Result<Coupon> Validate()
        {
            return new CouponValidator().Validate(this);
        }

        public void SetUsed(bool isUsed)
        {
            IsUsed = isUsed;
            if (IsUsed)
                UsedAt = DateTime.UtcNow;
            else
                UsedAt = null;
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new CouponDeletedEvent(Id));
        }
    }
}
