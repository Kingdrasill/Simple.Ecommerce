using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class UserPaymentMapping : IEntityTypeConfiguration<UserPayment>
    {
        public void Configure(EntityTypeBuilder<UserPayment> builder)
        {
            builder.ToTable("UsuariosPagamentos");

            builder.Property(up => up.Id).ValueGeneratedOnAdd();
            builder.HasKey(up => up.Id);

            builder.Property(up => up.UserId).IsRequired();

            builder.Property(up => up.Deleted).IsRequired();

            builder.OwnsOne(up => up.PaymentInformation, pi => 
            { 
                pi.Property(pi => pi.PaymentMethod).HasColumnName("PaymentMethod").IsRequired();
                pi.Property(pi => pi.PaymentName).HasColumnName("PaymentName").HasMaxLength(50);
                pi.Property(pi => pi.PaymentKey).HasColumnName("PaymentKey");
                pi.Property(pi => pi.ExpirationMonth).HasColumnName("ExpirationMonth").HasMaxLength(2);
                pi.Property(pi => pi.ExpirationYear).HasColumnName("ExpirationYear").HasMaxLength(5);
                pi.Property(pi => pi.CardFlag).HasColumnName("CardFlag");
                pi.Property(pi => pi.Last4Digits).HasColumnName("Last4Digits");
                pi.WithOwner();
            });
        }
    }
}
