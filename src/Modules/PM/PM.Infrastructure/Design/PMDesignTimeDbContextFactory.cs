using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.PM.Infrastructure.Design;

internal sealed class PMDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PMDbContext>
{
    public PMDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PMDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("PM_CONNECTION")
                               ?? "Host=localhost;Database=himapp_pm;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new PMDbContext(builder.Options);
    }
}
