using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;

namespace Flora.Cms.Infrastructure.Persistence.Configurations;

public class ApplicationConfig : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> b)
    {
        b.ToTable("Applications");
        b.HasKey(x => x.Id);

        b.Property(x => x.CandidateName).HasMaxLength(256).IsRequired();
        b.Property(x => x.CandidateEmail).HasMaxLength(256).IsRequired();
        b.Property(x => x.CandidatePhone).HasMaxLength(64);
        b.Property(x => x.ResumeUrl).HasMaxLength(2048).IsRequired();
        b.Property(x => x.CoverLetter).HasMaxLength(8000);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        b.HasOne(x => x.JobPosting)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.JobPostingId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => new { x.JobPostingId, x.Status });
    }
}
