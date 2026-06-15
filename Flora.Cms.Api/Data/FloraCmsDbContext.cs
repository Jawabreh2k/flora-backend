using Flora.Cms.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Api.Data;

public class FloraCmsDbContext(DbContextOptions<FloraCmsDbContext> options) : DbContext(options)
{
    public DbSet<UiConfiguration> UiConfigurations => Set<UiConfiguration>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<UiConfigRevision> UiConfigRevisions => Set<UiConfigRevision>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Application> Applications => Set<Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FloraCmsDbContext).Assembly);
    }
}
