using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.Store.Infrastructure.Design;

internal sealed class StoreDesignTimeDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<StoreDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("STORE_CONNECTION")
                               ?? "Host=localhost;Database=himapp_store;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new StoreDbContext(builder.Options);
    }
}
