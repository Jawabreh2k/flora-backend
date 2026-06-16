using System.Text.Json;
using Flora.Cms.Application.Features.UiConfig;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Common;
using Flora.Cms.Domain.Entities;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flora.Cms.Infrastructure.Persistence;

/// <summary>
/// Applies pending migrations and seeds the singleton UI configuration plus a
/// bootstrap admin user on startup. Seeding is idempotent.
/// </summary>
public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FloraCmsDbContext>();

        await db.Database.MigrateAsync();

        // 1) Seed the singleton UI configuration with the site's real brand defaults.
        if (!await db.UiConfigurations.AnyAsync(x => x.Id == UiConfiguration.SingletonId))
        {
            db.UiConfigurations.Add(new UiConfiguration
            {
                Id = UiConfiguration.SingletonId,
                PayloadJson = JsonSerializer.Serialize(UiConfigPayload.CreateDefault(), JsonDefaults.Payload),
            });
        }

        // 2) Seed a bootstrap admin if none exists. Credentials come from configuration
        //    (Seed:AdminUsername / Seed:AdminEmail / Seed:AdminPassword) so they are never
        //    hard-coded. The default password is intended for local development only.
        if (!await db.AdminUsers.AnyAsync())
        {
            var username = config["Seed:AdminUsername"] ?? "admin";
            var email = config["Seed:AdminEmail"] ?? "admin@floragroup.net";
            var password = config["Seed:AdminPassword"] ?? "ChangeMe!2026";

            db.AdminUsers.Add(new AdminUser
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11),
                Role = Roles.Admin,
            });
        }

        await db.SaveChangesAsync();

        // 3) Seed default company for job postings.
        if (!await db.Companies.AnyAsync())
        {
            var now = DateTimeOffset.UtcNow;
            var company = new Company
            {
                Name = "Flora Group",
                LogoUrl = "/images/flora-group-logo.png",
                Website = "https://floragroup.net",
                Description = "A diversified group of companies across hospitality, real estate, and services.",
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.Companies.Add(company);
            await db.SaveChangesAsync();

            db.JobPostings.AddRange(
                new JobPosting
                {
                    CompanyId = company.Id,
                    Title = "Senior Hospitality Manager",
                    Department = "Hospitality",
                    LocationType = LocationType.HYBRID,
                    LocationText = "Amman, Jordan",
                    JobType = JobType.FULL_TIME,
                    Description =
                        "Lead operational excellence across Flora hospitality assets, partner with subsidiary GMs, and drive guest satisfaction metrics.",
                    Requirements = ["5+ years in hospitality leadership", "Fluent in Arabic and English"],
                    Responsibilities =
                    [
                        "Own daily operations across assigned properties",
                        "Coach subsidiary leadership teams",
                        "Report KPIs to group executive committee",
                    ],
                    SalaryRange = "JOD 1,200 – 1,800",
                    Status = JobStatus.PUBLISHED,
                    ClosingDate = now.AddMonths(2),
                    CreatedAt = now,
                    UpdatedAt = now,
                },
                new JobPosting
                {
                    CompanyId = company.Id,
                    Title = "Digital Marketing Specialist",
                    Department = "Marketing",
                    LocationType = LocationType.REMOTE,
                    LocationText = "Remote · MENA",
                    JobType = JobType.FULL_TIME,
                    Description =
                        "Plan and execute multi-brand campaigns for Flora Group subsidiaries with a focus on measurable growth.",
                    Requirements = ["3+ years in digital marketing", "Meta & Google Ads certified"],
                    Responsibilities =
                    [
                        "Manage paid and organic campaigns",
                        "Collaborate with brand teams on content",
                        "Track and optimize conversion funnels",
                    ],
                    SalaryRange = "JOD 900 – 1,300",
                    Status = JobStatus.PUBLISHED,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            await db.SaveChangesAsync();
        }
    }
}
