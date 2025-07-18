using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.LoginEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class LoginMapping : IEntityTypeConfiguration<Login>
    {
        public void Configure(EntityTypeBuilder<Login> builder)
        {
            builder.ToTable("Logins");

            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.HasKey(l => l.Id);

            builder.Property(l => l.UserId).IsRequired();
            builder.Property(l => l.Credential).IsRequired();
            builder.Property(l => l.Password).IsRequired();
            builder.Property(l => l.Type).IsRequired();
            builder.Property(l => l.IsVerified).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();

            builder.HasIndex(l => l.Credential).IsUnique();

            builder
                .HasMany(l => l.CredentialVerifications)
                .WithOne(vc => vc.Login)
                .HasForeignKey(vc => vc.LoginId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
