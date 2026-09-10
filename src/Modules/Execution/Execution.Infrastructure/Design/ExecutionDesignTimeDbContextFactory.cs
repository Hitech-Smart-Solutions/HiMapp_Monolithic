using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.Execution.Infrastructure.Design;

internal sealed class ExecutionDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ExecutionDbContext>
{
    public ExecutionDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ExecutionDbContext>();
        // Default local development connection string - adjust as needed for your environment.
        var connectionString = Environment.GetEnvironmentVariable("EXECUTION_CONNECTION")
                               ?? "Database connection does not found";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new ExecutionDbContext(builder.Options);
    }
}
