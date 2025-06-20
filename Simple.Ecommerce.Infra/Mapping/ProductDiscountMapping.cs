using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class ProductDiscountMapping : IEntityTypeConfiguration<ProductDiscount>
    {
        public void Configure(EntityTypeBuilder<ProductDiscount> builder)
        {
            builder.ToTable("ProdutosDescontos");

            builder.Property(pd => pd.Id).ValueGeneratedOnAdd();
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.ProductId).IsRequired();
            builder.Property(pd => pd.DiscountId).IsRequired();

            builder.Property(pd => pd.Deleted).IsRequired();

            builder.HasOne(pd => pd.Product)
                .WithMany(p => p.ProductDiscounts)
                .HasForeignKey(pd => pd.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pd => pd.Discount)
                .WithMany(d => d.ProductDiscounts)
                .HasForeignKey(pd => pd.DiscountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
