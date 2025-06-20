using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class DiscountMapping : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Descontos");

            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name).HasMaxLength(30).IsRequired();
            builder.Property(d => d.DiscountType).IsRequired();
            builder.Property(d => d.DiscountScope).IsRequired();
            builder.Property(d => d.DiscountValueType);
            builder.Property(d => d.Value);
            builder.Property(d => d.ValidFrom);
            builder.Property(d => d.ValidTo);
            builder.Property(d => d.IsActive).IsRequired();   

            builder.Property(d => d.Deleted).IsRequired();

            builder
                .HasMany(d => d.DiscountTiers)
                .WithOne(dt => dt.Discount)
                .HasForeignKey(dt => dt.DiscountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.DiscountBundleItems)
                .WithOne(dbi => dbi.Discount)
                .HasForeignKey(dbi => dbi.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.Coupons)
                .WithOne(c => c.Discount)
                .HasForeignKey(c => c.DiscountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
