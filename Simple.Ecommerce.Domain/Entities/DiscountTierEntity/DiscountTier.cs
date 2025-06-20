using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.DiscountTierEntity
{
    public class DiscountTier : BaseEntity
    {
        public int MinQuantity { get; private set; }
        public decimal Value { get; private set; }
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;

        public DiscountTier() { }
    
        private DiscountTier(int id, int minQuantity, decimal value, int discountId)
        {
            Id = id; 
            MinQuantity = minQuantity;
            Value = value;
            DiscountId = discountId;
        }

        public Result<DiscountTier> Create(int id, int minQuantity, decimal value, int discountId) 
        {
            return new DiscountTierValidator().Validate(new DiscountTier(id, minQuantity, value, discountId));
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
