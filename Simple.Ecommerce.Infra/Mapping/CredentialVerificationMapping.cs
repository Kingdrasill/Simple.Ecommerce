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

            builder.Property(vc => vc.Id).ValueGeneratedOnAdd();
            builder.HasKey(vc => vc.Id);

            builder.Property(vc => vc.Token).IsRequired();
            builder.Property(vc => vc.ExpiresAt).IsRequired();
            builder.Property(vc => vc.IsUsed).IsRequired();
            builder.Property(vc => vc.LoginId).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();
        }
    }
}
