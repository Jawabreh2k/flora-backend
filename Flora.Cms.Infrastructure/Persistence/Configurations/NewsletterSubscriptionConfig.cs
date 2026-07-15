using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class NewsletterSubscriptionConfig : IEntityTypeConfiguration<NewsletterSubscription>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscription> b)
    {
        b.ToTable("NewsletterSubscriptions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.Locale).HasMaxLength(8).IsRequired();
        // The subscribe handler is check-then-insert; this index is the backstop
        // that guarantees no duplicates under concurrent submits.
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.CreatedAtUtc);
    }
}
