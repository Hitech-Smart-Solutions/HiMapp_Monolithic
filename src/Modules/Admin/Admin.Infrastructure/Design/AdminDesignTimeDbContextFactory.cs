using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.Admin.Infrastructure.Design;

internal sealed class AdminDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AdminDbContext>
{
    public AdminDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AdminDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ADMIN_CONNECTION")
                               ?? "Host=localhost;Database=himapp_admin;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new AdminDbContext(builder.Options);
    }
}
