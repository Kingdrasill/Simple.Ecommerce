using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class ProductCategoryMapping : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProdutosCategorias");

            builder.Property(pc => pc.Id).ValueGeneratedOnAdd();
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.ProductId).IsRequired();
            builder.Property(pc => pc.CategoryId).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();

            builder.HasIndex(pc => new { pc.ProductId, pc.CategoryId }).IsUnique();

            builder.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            builder.HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
