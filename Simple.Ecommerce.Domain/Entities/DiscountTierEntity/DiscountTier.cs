using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;

namespace Simple.Ecommerce.Domain.Entities.DiscountTierEntity
{
    public class DiscountTier : BaseEntity
    {
        public string Name { get; private set; }
        public int MinQuantity { get; private set; }
        public decimal Value { get; private set; }
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;

        public DiscountTier() { }
    
        private DiscountTier(int id, string name, int minQuantity, decimal value, int discountId)
        {
            Id = id; 
            Name = name;
            MinQuantity = minQuantity;
            Value = value;
            DiscountId = discountId;
        }

        public Result<DiscountTier> Create(int id, string name, int minQuantity, decimal value, int discountId) 
        {
            return new DiscountTierValidator().Validate(new DiscountTier(id, name, minQuantity, value, discountId));
        }

        public Result<DiscountTier> Validate()
        {
            return new DiscountTierValidator().Validate(this);
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new DiscountTierDeletedEvent(Id));
        }
    }
}
