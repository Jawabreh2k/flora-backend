using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class CompanyConfig : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> b)
    {
        b.ToTable("Companies");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.LogoUrl).HasMaxLength(2048);
        b.Property(x => x.Website).HasMaxLength(2048);
        b.Property(x => x.Description).HasMaxLength(4000);
    }
}
