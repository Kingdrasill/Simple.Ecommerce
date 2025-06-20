using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class OrderDiscountMapping : IEntityTypeConfiguration<OrderDiscount>
    {
        public void Configure(EntityTypeBuilder<OrderDiscount> builder)
        {
            builder.ToTable("PedidosDescontos");

            builder.Property(pd => pd.Id).ValueGeneratedOnAdd();
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.OrderId).IsRequired();
            builder.Property(pd => pd.DiscountId).IsRequired();

            builder.Property(pd => pd.Deleted).IsRequired();

            builder.HasOne(pd => pd.Order)
                .WithMany(p => p.OrderDiscounts)
                .HasForeignKey(pd => pd.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pd => pd.Discount)
                .WithMany(d => d.OrderDiscounts)
                .HasForeignKey(pd => pd.DiscountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
