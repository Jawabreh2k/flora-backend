using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class ContactInquiryConfig : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> b)
    {
        b.ToTable("ContactInquiries");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.Company).HasMaxLength(256);
        b.Property(x => x.Subject).HasMaxLength(512).IsRequired();
        b.Property(x => x.Message).HasMaxLength(8000).IsRequired();
        b.Property(x => x.Locale).HasMaxLength(8).IsRequired();
        b.HasIndex(x => x.CreatedAtUtc);
    }
}
