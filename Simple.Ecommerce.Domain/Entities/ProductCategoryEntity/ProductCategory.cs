using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;

namespace Simple.Ecommerce.Domain.Entities.ProductCategoryEntity
{
    public class ProductCategory : BaseEntity
    {
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;

        public ProductCategory() { }

        private ProductCategory(int id, int productId, int categoryId) 
        {
            Id = id;
            ProductId = productId;
            CategoryId = categoryId;
        }

        public Result<ProductCategory> Create(int id, int productId, int categoryId) 
        {
            return new ProductCategoryValidator().Validate(new ProductCategory(id, productId, categoryId));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new ProductCategoryDeletedEvent(Id));
        }
    }
}
