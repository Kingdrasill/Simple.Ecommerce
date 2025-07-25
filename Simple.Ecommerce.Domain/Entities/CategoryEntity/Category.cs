using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.CategoryEntity
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<ProductCategory> ProductCategories { get; private set; }

        public Category()
        {
            ProductCategories = new HashSet<ProductCategory>();
        }

        private Category(int id, string name)
        {
            Id = id;
            Name = name;

            ProductCategories = new HashSet<ProductCategory>();
        }

        public Result<Category> Create(int id, string name)
        {
            return new CategoryValidator().Validate(new Category(id, name));
        }

        public Result<Category> Validate()
        {
            return new CategoryValidator().Validate(this);
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new CategoryDeletedEvent(Id));
        }
    }
}
