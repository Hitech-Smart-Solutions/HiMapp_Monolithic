using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Himapp.Workflow.Infrastructure.Design;

internal sealed class WorkflowDesignTimeDbContextFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<WorkflowDbContext>();
        // Default local development connection string - adjust as needed for your environment.
        var connectionString = Environment.GetEnvironmentVariable("WORKFLOW_CONNECTION")
                               ?? "Host=localhost;Database=himapp_workflow;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString, b => b.UseRelationalNulls());

        return new WorkflowDbContext(builder.Options);
    }
}
