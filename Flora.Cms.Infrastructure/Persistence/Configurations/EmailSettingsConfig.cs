using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class EmailSettingsConfig : IEntityTypeConfiguration<EmailSettings>
{
    public void Configure(EntityTypeBuilder<EmailSettings> b)
    {
        b.ToTable("EmailSettings");

        b.HasKey(x => x.Id);
        // Singleton row: the key is controlled explicitly, never auto-generated.
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Provider).HasConversion<string>().HasMaxLength(16).IsRequired();
        b.Property(x => x.FromAddress).HasMaxLength(256).IsRequired();
        b.Property(x => x.SmtpHost).HasMaxLength(256);
        b.Property(x => x.SmtpUser).HasMaxLength(256);
        b.Property(x => x.SmtpPasswordEncrypted).HasMaxLength(2048);
        b.Property(x => x.ResendApiKeyEncrypted).HasMaxLength(2048);
        b.Property(x => x.UpdatedBy).HasMaxLength(256);
    }
}
