using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class CategoryMapping : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categorias");

            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.HasKey(c => c.Id);

            builder.Property(f => f.Deleted).IsRequired();

            builder.Property(c => c.Name).HasMaxLength(30).IsRequired();
        }
    }
}
