using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class AdminUserConfig : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> b)
    {
        b.ToTable("AdminUsers");

        b.HasKey(x => x.Id);

        b.Property(x => x.Username).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.Username).IsUnique();

        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
        b.Property(x => x.Role).HasMaxLength(64).IsRequired();
    }
}
