using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;

namespace Simple.Ecommerce.Domain.Entities.ProductPhotoEntity
{
    public class ProductPhoto : BaseEntity
    {
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public Photo Photo { get; private set; }

        public ProductPhoto() { }

        private ProductPhoto(int id, int productId, Photo photo)
        {
            Id = id;
            ProductId = productId;
            Photo = photo;
        }

        public Result<ProductPhoto> Create(int id, int productId, Photo photo)
        {
            return new ProductPhotoValidator().Validate(new ProductPhoto(id, productId, photo));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new ProductPhotoDeletedEvent(Id));
        }
    }
}
