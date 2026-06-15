using Flora.Cms.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Api.Data.Configurations;

public class UiConfigurationConfig : IEntityTypeConfiguration<UiConfiguration>
{
    public void Configure(EntityTypeBuilder<UiConfiguration> b)
    {
        b.ToTable("UiConfigurations");

        b.HasKey(x => x.Id);
        // Singleton row: do NOT auto-generate the key, we control it explicitly.
        b.Property(x => x.Id).ValueGeneratedNever();

        // The whole point of the design: one unbounded JSON column, no per-field schema.
        b.Property(x => x.PayloadJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        b.Property(x => x.Revision).IsConcurrencyToken(false);

        b.Property(x => x.UpdatedBy).HasMaxLength(256);

        // SQL Server optimistic-concurrency token.
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}
