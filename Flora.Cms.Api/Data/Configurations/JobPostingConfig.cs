using Flora.Cms.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flora.Cms.Api.Data.Configurations;

public class JobPostingConfig : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> b)
    {
        b.ToTable("JobPostings");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).HasMaxLength(256).IsRequired();
        b.Property(x => x.Department).HasMaxLength(128).IsRequired();
        b.Property(x => x.LocationText).HasMaxLength(256).IsRequired();
        b.Property(x => x.Description).IsRequired();
        b.Property(x => x.SalaryRange).HasMaxLength(128);
        b.Property(x => x.LocationType).HasConversion<string>().HasMaxLength(32);
        b.Property(x => x.JobType).HasConversion<string>().HasMaxLength(32);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        b.Property(x => x.Requirements)
            .HasConversion(JsonStringListConverter.Instance)
            .Metadata.SetValueComparer(JsonStringListConverter.Comparer);

        b.Property(x => x.Responsibilities)
            .HasConversion(JsonStringListConverter.Instance)
            .Metadata.SetValueComparer(JsonStringListConverter.Comparer);

        b.HasOne(x => x.Company)
            .WithMany(x => x.JobPostings)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CompanyId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.Department);
        b.HasIndex(x => new { x.Status, x.Department });
    }
}
