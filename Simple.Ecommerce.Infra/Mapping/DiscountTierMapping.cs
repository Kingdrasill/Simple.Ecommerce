using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class DiscountTierMapping : IEntityTypeConfiguration<DiscountTier>
    {
        public void Configure(EntityTypeBuilder<DiscountTier> builder)
        {
            builder.ToTable("DescontoTiers");

            builder.Property(dt => dt.Id).ValueGeneratedOnAdd();
            builder.HasKey(dt => dt.Id);

            builder.Property(dt => dt.MinQuantity).IsRequired();
            builder.Property(dt => dt.Value).IsRequired();
            builder.Property(dt => dt.DiscountId).IsRequired();

            builder.Property(dt => dt.Deleted);
        }
    }
}
