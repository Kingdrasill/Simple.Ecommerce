using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;

namespace Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity
{
    public class DiscountBundleItem : BaseEntity
    {
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int Quantity { get; private set; }

        public DiscountBundleItem() { }

        private DiscountBundleItem(int id, int discountId, int productId, int quantity)
        {
            Id = id;
            DiscountId = discountId;
            ProductId = productId;
            Quantity = quantity;
        }

        public Result<DiscountBundleItem> Create(int id, int discountId, int productId, int quantity)
        {
            return new DiscountBundleItemValidator().Validate(new DiscountBundleItem(id, discountId, productId, quantity));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new DiscountBundleItemDeletedEvent(Id));
        }
    }
}
