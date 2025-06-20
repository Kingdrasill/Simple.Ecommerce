using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    internal class DiscountBundleItemMapping : IEntityTypeConfiguration<DiscountBundleItem>
    {
        public void Configure(EntityTypeBuilder<DiscountBundleItem> builder)
        {
            builder.ToTable("DescontoItensPacote");

            builder.Property(dbi => dbi.Id).ValueGeneratedOnAdd();
            builder.HasKey(dbi => dbi.Id);

            builder.Property(dbi => dbi.DiscountId).IsRequired();
            builder.Property(dbi => dbi.ProductId).IsRequired();
            builder.Property(dbi => dbi.Quantity).IsRequired();

            builder.Property(dbi => dbi.Deleted).IsRequired();
        }
    }
}
