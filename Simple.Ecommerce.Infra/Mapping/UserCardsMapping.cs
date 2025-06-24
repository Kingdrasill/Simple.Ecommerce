using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class UserCardsMapping : IEntityTypeConfiguration<UserCard>
    {
        public void Configure(EntityTypeBuilder<UserCard> builder)
        {
            builder.ToTable("UsuariosCartoes");

            builder.Property(uc => uc.Id).ValueGeneratedOnAdd();
            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.UserId).IsRequired();

            builder.Property(uc => uc.Deleted).IsRequired();

            builder.OwnsOne(uc => uc.CardInformation, ci => 
            { 
                ci.Property(ci => ci.CardHolderName).HasColumnName("CardHolderName").HasMaxLength(50).IsRequired();
                ci.Property(ci => ci.CardNumber).HasColumnName("CardNumber").IsRequired();
                ci.Property(ci => ci.ExpirationMonth).HasColumnName("ExpirationMonth").HasMaxLength(2).IsRequired();
                ci.Property(ci => ci.ExpirationYear).HasColumnName("ExpirationYear").HasMaxLength(5).IsRequired();
                ci.Property(ci => ci.CardFlag).HasColumnName("CardFlag").IsRequired();
                ci.Property(ci => ci.Last4Digits).HasColumnName("Last4Digits").IsRequired();
                ci.WithOwner();
            });
        }
    }
}
