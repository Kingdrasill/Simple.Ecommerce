using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Maooing
{
    public class OrderMaooing : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Pedidos");

            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderType).IsRequired();
            builder.Property(o => o.PaymentMethod);
            builder.Property(o => o.TotalPrice);
            builder.Property(o => o.OrderDate);
            builder.Property(o => o.Confirmation).IsRequired();
            builder.Property(o => o.Status).HasMaxLength(20).IsRequired();
            builder.Property(o => o.DiscountId);

            builder.Property(f => f.Deleted).IsRequired();

            builder.OwnsOne(o => o.Address, a =>
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

            builder.OwnsOne(o => o.CardInformation, ci =>
            {
                ci.Property(ci => ci.CardHolderName).HasColumnName("CardHolderName");
                ci.Property(ci => ci.CardNumber).HasColumnName("CardNumber");
                ci.Property(ci => ci.ExpirationMonth).HasColumnName("ExpirationMonth");
                ci.Property(ci => ci.ExpirationYear).HasColumnName("ExpirationYear");
                ci.Property(ci => ci.CardFlag).HasColumnName("CardFlag");
                ci.Property(ci => ci.Last4Digits).HasColumnName("Last4Digits");
                ci.WithOwner();
            });

            builder
                .HasMany(o => o.OrderItems)
                .WithOne(ci => ci.Order)
                .HasForeignKey(ci => ci.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
