using Microsoft.EntityFrameworkCore;
using Himapp.Execution.Domain.Entities;

namespace Himapp.Execution.Infrastructure;

public sealed class ExecutionDbContext : DbContext
{
    public ExecutionDbContext(DbContextOptions<ExecutionDbContext> options) : base(options)
    {
    }

    // DbSets for execution domain entities
    public DbSet<UOM> UOMs { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
    public DbSet<RateMaster> RateMasters { get; set; } = null!;
    public DbSet<Planning> Plannings { get; set; } = null!;
    public DbSet<PlanningDetail> PlanningDetails { get; set; } = null!;
    public DbSet<DailyProgress> DailyProgresses { get; set; } = null!;
    public DbSet<DailyProgressDetail> DailyProgressDetails { get; set; } = null!;
    public DbSet<DailyDepartmentalLabourSlip> DailyLabors { get; set; } = null!;
    public DbSet<DailyDepartmentalLabourSlipDetails> DailyLaborDetails { get; set; } = null!;
    public DbSet<Manpower> Manpowers { get; set; } = null!;
    public DbSet<ManpowerDetail> ManpowerDetails { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Additional configuration can be provided via IEntityTypeConfiguration implementations in domain project
    }
}
