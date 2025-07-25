using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class CredentialVerificationMapping : IEntityTypeConfiguration<CredentialVerification>
    {
        public void Configure(EntityTypeBuilder<CredentialVerification> builder)
        {
            builder.ToTable("VerificacaoCredenciais");

            builder.Property(cv => cv.Id).ValueGeneratedOnAdd();
            builder.HasKey(cv => cv.Id);

            builder.Property(cv => cv.Token).IsRequired();
            builder.Property(cv => cv.ExpiresAt).IsRequired();
            builder.Property(cv => cv.IsUsed).IsRequired();
            builder.Property(cv => cv.LoginId).IsRequired();
            builder.Property(cv => cv.UsedAt);

            builder.Property(f => f.Deleted).IsRequired();
        }
    }
}
