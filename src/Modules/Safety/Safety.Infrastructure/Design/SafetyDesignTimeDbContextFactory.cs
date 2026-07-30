using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.Safety.Infrastructure.Design;

internal sealed class SafetyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SafetyDbContext>
{
    public SafetyDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SafetyDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("SAFETY_CONNECTION")
                               ?? "Host=localhost;Database=himapp_safety;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new SafetyDbContext(builder.Options);
    }
}
