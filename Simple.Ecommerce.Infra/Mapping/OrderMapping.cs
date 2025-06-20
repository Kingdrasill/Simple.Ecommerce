using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class OrderMapping : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Pedidos");

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderDate).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.TotalPrice).IsRequired();
            builder.Property(p => p.OrderType).IsRequired();
            builder.Property(p => p.Confirmation).IsRequired();
            builder.Property(p => p.Status).HasMaxLength(20).IsRequired();
            builder.Property(p => p.PaymentMethod).HasMaxLength(20);

            builder.Property(f => f.Deleted).IsRequired();

            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(a => a.Number).HasColumnName("Number").IsRequired();
                a.Property(a => a.Street).HasColumnName("Street").HasMaxLength(30).IsRequired();
                a.Property(a => a.Neighbourhood).HasColumnName("Neighbourhood").HasMaxLength(30).IsRequired();
                a.Property(a => a.City).HasColumnName("City").HasMaxLength(30).IsRequired();
                a.Property(a => a.Country).HasColumnName("Country").HasMaxLength(30).IsRequired();
                a.Property(a => a.Complement).HasColumnName("Complement").HasMaxLength(30);
                a.Property(a => a.CEP).HasColumnName("CEP").HasMaxLength(8).IsRequired();
                a.WithOwner();
            });

            builder
                .HasMany(p => p.OrderItems)
                .WithOne(ci => ci.Order)
                .HasForeignKey(ci => ci.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
