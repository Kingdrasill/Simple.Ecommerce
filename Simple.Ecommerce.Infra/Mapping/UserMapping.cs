using Simple.Ecommerce.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Usuarios");

            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.HasKey(c => c.Id);

            builder.Property(o => o.Name).HasMaxLength(50).IsRequired();
            builder.Property(o => o.Email).HasMaxLength(50).IsRequired();
            builder.Property(o => o.PhoneNumber).HasMaxLength(14).IsRequired();
            builder.Property(o => o.Password).IsRequired();

            builder.Property(f => f.Deleted).IsRequired();

            builder.OwnsOne(o => o.Photo, p =>
            {
                p.Property(p => p.FileName).HasColumnName("FileName");
                p.WithOwner();
            });

            builder.HasIndex(o => o.Email).IsUnique();
            builder.HasIndex(o => o.PhoneNumber).IsUnique();

            builder
                .HasMany(u => u.UserAddresses)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.UserCards)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Logins)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .IsRequired()
                .OnDelete (DeleteBehavior.Cascade);
        }
    }
}
