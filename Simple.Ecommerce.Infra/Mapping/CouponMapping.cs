using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.CouponEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class CouponMapping : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Cupons");

            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired();
            builder.Property(c => c.IsUsed).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.ExpirationAt).IsRequired();
            builder.Property(c => c.UsedAt);
            builder.Property(c => c.DiscountId).IsRequired();

            builder.Property(c => c.Deleted).IsRequired();

            builder.HasIndex(c => c.Code).IsUnique();

            builder
                .HasMany(c => c.Orders)
                .WithOne(o => o.Coupon)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(c => c.OrderItems)
                .WithOne(oi => oi.Coupon)
                .HasForeignKey(oi => oi.CouponId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
