using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class UserAddressMapping : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("UsuarioEnderecos");

            builder.Property(ue => ue.Id).ValueGeneratedOnAdd();
            builder.HasKey(ue => ue.Id);

            builder.Property(ue => ue.UserId).IsRequired();
            
            builder.Property(f => f.Deleted).IsRequired();

            builder.OwnsOne(ue => ue.Address, a =>
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
        }
    }
}
