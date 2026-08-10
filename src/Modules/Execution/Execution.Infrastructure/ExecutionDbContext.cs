using Microsoft.EntityFrameworkCore;
using Himapp.Execution.Domain.Entities;
using System.Reflection;
using Himapp.Execution.Application;

namespace Himapp.Execution.Infrastructure;

public sealed class ExecutionDbContext : DbContext, IExecutionDbContext
{
    public ExecutionDbContext(DbContextOptions<ExecutionDbContext> options) : base(options)
    {
    }

    // DbSets for execution domain entities
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<ActivityCategoryDetails> ActivityCategoryDetails { get; set; } = null!;
    public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
    public DbSet<Planning> Plannings { get; set; } = null!;
    public DbSet<PlanningDetail> PlanningDetails { get; set; } = null!;
    public DbSet<PlanningDocumentDetail> PlanningDocumentDetail { get; set; } = null!;
    public DbSet<DailyProgress> DailyProgresses { get; set; } = null!;
    public DbSet<DailyProgressDetail> DailyProgressDetails { get; set; } = null!;
    public DbSet<DailyLabor> DailyLabors { get; set; } = null!;
    public DbSet<DailyLaborDetail> DailyLaborDetails { get; set; } = null!;
    public DbSet<Manpower> Manpowers { get; set; } = null!;
    public DbSet<ManpowerDetail> ManpowerDetails { get; set; } = null!;
    public DbSet<DailyDepartmentalLabourSlip> DailyDepartmentalLabourSlips { get; set; } = null!;
    public DbSet<SiteDailyProgress> SiteDailyProgresses { get; set; } = null!;
    public DbSet<ExecutionProjectConfig> ExecutionProjectConfigs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("execution"); // your schema name
        base.OnModelCreating(modelBuilder);
        // Load IEntityTypeConfiguration implementations from the domain assembly so fluent configurations
        // placed in Execution.Domain are applied automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DailyLabor).Assembly);
    }
}
