using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("PedidosItens");

            builder.Property(oi => oi.Id).ValueGeneratedOnAdd();
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Price).IsRequired();
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.ProductId).IsRequired();
            builder.Property(oi => oi.OrderId).IsRequired();
            builder.Property(oi => oi.DiscountId);

            builder.Property(oi => oi.Deleted).IsRequired();
        }
    }
}
