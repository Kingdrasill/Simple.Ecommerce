using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("ItensPedido");

            builder.Property(ci => ci.Id).ValueGeneratedOnAdd();
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Price).IsRequired();
            builder.Property(ci => ci.Quantity).IsRequired();
            builder.Property(ci => ci.ProductId).IsRequired();
            builder.Property(ci => ci.OrderId).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();

            builder.HasIndex(ci => new { ci.OrderId, ci.ProductId}).IsUnique();
        }
    }
}
