using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class UiConfigRevisionConfig : IEntityTypeConfiguration<UiConfigRevision>
{
    public void Configure(EntityTypeBuilder<UiConfigRevision> b)
    {
        b.ToTable("UiConfigRevisions");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();

        b.Property(x => x.PayloadJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        b.Property(x => x.SavedBy).HasMaxLength(256);

        // Each (ConfigId, Revision) pair is unique — prevents duplicate snapshots.
        b.HasIndex(x => new { x.ConfigId, x.Revision }).IsUnique();
    }
}
