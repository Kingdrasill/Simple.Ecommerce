using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.ProductEntity
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string Description { get; private set; }
        public int Stock { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<ProductPhoto> ProductPhotos { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<ProductCategory> ProductCategories { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Review> Reviews { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<DiscountBundleItem> DiscountBundleItems { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<ProductDiscount> ProductDiscounts { get; private set; }

        public Product() 
        {
            ProductPhotos = new HashSet<ProductPhoto>();
            ProductCategories = new HashSet<ProductCategory>();
            Reviews = new HashSet<Review>();
            OrderItems = new HashSet<OrderItem>();
            DiscountBundleItems = new HashSet<DiscountBundleItem>();
            ProductDiscounts = new HashSet<ProductDiscount>();
        }

        private Product(int id, string name, decimal price, string description, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            Stock = stock;

            ProductPhotos = new HashSet<ProductPhoto>();
            ProductCategories = new HashSet<ProductCategory>();
            Reviews = new HashSet<Review>();
            OrderItems = new HashSet<OrderItem>();
            DiscountBundleItems = new HashSet<DiscountBundleItem>();
            ProductDiscounts = new HashSet<ProductDiscount>();
        }

        public Result<Product> Create(int id, string name, decimal price, string description, int stock)
        {
            return new ProductValidator().Validate(new Product(id, name, price, description, stock));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new ProductDeletedEvent(Id));
        }
    }
}
