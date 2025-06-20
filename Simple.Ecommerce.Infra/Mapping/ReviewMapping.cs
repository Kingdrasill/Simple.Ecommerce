using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class ReviewMapping : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Avaliacoes");

            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.UserId).IsRequired();
            builder.Property(r => r.Score).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(200);

            builder.Property(f => f.Deleted).IsRequired();

            builder.HasIndex(r => new { r.ProductId, r.UserId }).IsUnique();
        }
    }
}
