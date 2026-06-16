using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Security;
using Flora.Cms.Application.Abstractions.Storage;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Options;
using Flora.Cms.Infrastructure.Persistence;
using Flora.Cms.Infrastructure.Security;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Infrastructure.Email;
using Flora.Cms.Infrastructure.Storage;
using Flora.Cms.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flora.Cms.Infrastructure;

/// <summary>Composition root for the Infrastructure layer (persistence + adapters).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<FloraCmsDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("Default"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(FloraCmsDbContext).Assembly.FullName);
                    // Survive brief connectivity blips (LocalDB spin-up, SQL Azure throttling).
                    sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                }));

        // Expose the DbContext to the Application layer through its port.
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<FloraCmsDbContext>());

        services
            .AddOptions<ContactOptions>()
            .Bind(config.GetSection(ContactOptions.SectionName));

        services
            .AddOptions<JwtOptions>()
            .Bind(config.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        if (config.GetValue<bool>("Storage:UseS3"))
            services.AddScoped<IFileStorage, S3FileStorageService>();
        else
            services.AddScoped<IFileStorage, LocalFileStorageService>();

        if (!string.IsNullOrWhiteSpace(config[$"{ContactOptions.SectionName}:ResendApiKey"]))
            services.AddScoped<IEmailSender, ResendEmailSender>();
        else
            services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
