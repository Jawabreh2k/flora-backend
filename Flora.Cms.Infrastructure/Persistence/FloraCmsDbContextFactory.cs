using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flora.Cms.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core CLI (<c>dotnet ef migrations add</c> /
/// <c>database update</c>). Lets the tooling construct the context directly instead of
/// booting the API host, so migration commands never run the app's startup/seed logic.
/// The connection string here is only used at design time.
/// </summary>
public sealed class FloraCmsDbContextFactory : IDesignTimeDbContextFactory<FloraCmsDbContext>
{
    public FloraCmsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FloraCmsDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=FloraCms;Trusted_Connection=True;TrustServerCertificate=True;",
                sql => sql.MigrationsAssembly(typeof(FloraCmsDbContext).Assembly.FullName))
            .Options;

        return new FloraCmsDbContext(options);
    }
}
