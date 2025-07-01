using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;

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

        public Coupon() { }

        private Coupon(int id, string code, DateTime expirationAt, int discountId, bool isUsed = false, DateTime? usedAt = null, DateTime? createdAt = null)
        {
            Id = id; 
            Code = code;
            IsUsed = isUsed;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            ExpirationAt = expirationAt;
            UsedAt = usedAt;
            DiscountId = discountId;
        }

        public Result<Coupon> Create(int id, string code, DateTime expirationAt, int discountId, bool isUsed = false, DateTime? usedAt = null, DateTime? createdAt = null)
        {
            return new CouponValidator().Validate(new Coupon(id, code, expirationAt, discountId, isUsed, usedAt, createdAt));
        }

        public void SetAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
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
