using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;

namespace Flora.Cms.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IAppDbContext"/>. The Application layer depends
/// only on the interface; the SQL Server provider, entity configurations, and
/// migrations all live here in Infrastructure.
/// </summary>
public class FloraCmsDbContext(DbContextOptions<FloraCmsDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<UiConfiguration> UiConfigurations => Set<UiConfiguration>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<UiConfigRevision> UiConfigRevisions => Set<UiConfigRevision>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();
    public DbSet<EmailSettings> EmailSettings => Set<EmailSettings>();
    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FloraCmsDbContext).Assembly);
    }
}
