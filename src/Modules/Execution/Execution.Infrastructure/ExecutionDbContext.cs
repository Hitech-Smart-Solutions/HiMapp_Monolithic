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
    public DbSet<DailyProgressHindrance> DailyProgressHindrances { get; set; } = null!;
    public DbSet<DailyProgressPhoto> DailyProgressPhotos { get; set; } = null!;
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

        // ============================================================
        // DailyProgress
        // ============================================================

        modelBuilder.Entity<DailyProgress>(entity =>
        {
            entity.ToTable("DailyProgress");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.DPRCode)
                .HasColumnName("DPRCode");

            entity.Property(x => x.ProjectID)
                .IsRequired();

            entity.Property(x => x.ReportDate)
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

        });


        // ============================================================
        // DailyProgressDetails
        // ============================================================

        modelBuilder.Entity<DailyProgressDetail>(entity =>
        {
            entity.ToTable("DailyProgressDetails");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.DailyProgressID)
                .IsRequired();

            entity.Property(x => x.ActivityID)
                .IsRequired();

            entity.Property(x => x.Quantity)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.Rate)
                .HasPrecision(18, 2)
                .IsRequired();

            // Computed stored column in PostgreSQL
            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.PlanQuantity)
                .HasPrecision(18, 2);

            // Computed stored column in PostgreSQL
            entity.Property(x => x.Variance)
                .HasPrecision(18, 2);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.DailyProgress)
                .WithMany(x => x.DailyProgressDetail)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // ============================================================
        // DailyProgressHindrances
        // ============================================================

        modelBuilder.Entity<DailyProgressHindrance>(entity =>
        {
            entity.ToTable("DailyProgressHindrances");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.DailyProgressID)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.DailyProgress)
                .WithMany(x => x.DailyProgressHindrance)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // ============================================================
        // DailyProgressPhotos
        // ============================================================

        modelBuilder.Entity<DailyProgressPhoto>(entity =>
        {
            entity.ToTable("DailyProgressPhotos");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.DailyProgressID)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.DailyProgress)
                .WithMany(x => x.DailyProgressPhoto)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
