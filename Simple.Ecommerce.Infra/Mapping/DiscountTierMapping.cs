using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class DiscountTierMapping : IEntityTypeConfiguration<DiscountTier>
    {
        public void Configure(EntityTypeBuilder<DiscountTier> builder)
        {
            builder.ToTable("DescontosTiers");

            builder.Property(dt => dt.Id).ValueGeneratedOnAdd();
            builder.HasKey(dt => dt.Id);

            builder.Property(dt => dt.Name).HasMaxLength(30).IsRequired();
            builder.Property(dt => dt.MinQuantity).IsRequired();
            builder.Property(dt => dt.Value).IsRequired();
            builder.Property(dt => dt.DiscountId).IsRequired();

            builder.Property(dt => dt.Deleted);
        }
    }
}
