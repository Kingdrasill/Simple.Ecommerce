using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    internal class DiscountBundleItemMapping : IEntityTypeConfiguration<DiscountBundleItem>
    {
        public void Configure(EntityTypeBuilder<DiscountBundleItem> builder)
        {
            builder.ToTable("DescontosItensPacote");

            builder.Property(dbi => dbi.Id).ValueGeneratedOnAdd();
            builder.HasKey(dbi => dbi.Id);

            builder.Property(dbi => dbi.DiscountId).IsRequired();
            builder.Property(dbi => dbi.ProductId).IsRequired();
            builder.Property(dbi => dbi.Quantity).IsRequired();

            builder.Property(dbi => dbi.Deleted).IsRequired();
        }
    }
}
