using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class CacheFrequencyMapping : IEntityTypeConfiguration<CacheFrequency>
    {
        public void Configure(EntityTypeBuilder<CacheFrequency> builder)
        {
            builder.ToTable("Frequencias");

            builder.Property(f => f.Id).ValueGeneratedOnAdd();
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Entity).IsRequired();
            builder.Property(f => f.Frequency).IsRequired();
            builder.Property(f => f.HoursToLive);
            builder.Property(f => f.Expirable).IsRequired();
            builder.Property(f => f.KeepCached).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();
        }
    }
}
