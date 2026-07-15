using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
// The project namespace (Flora.Cms.Application) shadows the Application entity, so alias it.
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;

namespace Flora.Cms.Application.Abstractions.Persistence;

/// <summary>
/// Persistence port the Application layer queries against. Exposing <see cref="DbSet{T}"/>
/// keeps read handlers able to compose efficient, provider-translated LINQ while the
/// concrete <c>DbContext</c> (and its SQL Server provider) stays in Infrastructure —
/// inverting the dependency without hiding EF behind a per-aggregate repository.
/// </summary>
public interface IAppDbContext
{
    DbSet<UiConfiguration> UiConfigurations { get; }
    DbSet<UiConfigRevision> UiConfigRevisions { get; }
    DbSet<AdminUser> AdminUsers { get; }
    DbSet<Company> Companies { get; }
    DbSet<JobPosting> JobPostings { get; }
    DbSet<ApplicationEntity> Applications { get; }
    DbSet<ContactInquiry> ContactInquiries { get; }
    DbSet<EmailSettings> EmailSettings { get; }
    DbSet<NewsletterSubscription> NewsletterSubscriptions { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
