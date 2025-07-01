using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;

namespace Simple.Ecommerce.Domain.Entities.ProductDiscountEntity
{
    public class ProductDiscount : BaseEntity
    {
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;

        public ProductDiscount() { }

        private ProductDiscount(int id, int productId, int discountId)
        {
            Id = id;
            ProductId = productId;
            DiscountId = discountId;
        }

        public Result<ProductDiscount> Create(int id, int productId, int discountId)
        {
            return new ProductDiscountValidator().Validate(new ProductDiscount(id, productId, discountId));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new ProductDiscountDeletedEvent(Id));
        }
    }
}
