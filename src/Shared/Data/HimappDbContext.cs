using Microsoft.EntityFrameworkCore;
using Himapp.Notifications.Models;
using Himapp.Audit.Models;
using Himapp.Execution.Domain.Entities;

namespace Himapp.Data;

public sealed class HimappDbContext : DbContext
{
    public HimappDbContext(DbContextOptions<HimappDbContext> options) : base(options) { }

    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationDeliveryLog> NotificationDeliveryLogs => Set<NotificationDeliveryLog>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    // Execution module entities (added to support Execution APIs)
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ProjectActivity> ProjectActivities => Set<ProjectActivity>();
    public DbSet<Area> Areas => Set<Area>();
    // UOM and RateMaster / Planning / Manpower / DailyProgress tables
    public DbSet<RateMaster> RateMasters => Set<RateMaster>();
    public DbSet<Planning> Plannings => Set<Planning>();
    public DbSet<PlanningDetails> PlanningDetails => Set<PlanningDetails>();
    public DbSet<Manpower> Manpowers => Set<Manpower>();
    public DbSet<ManpowerDetails> ManpowerDetails => Set<ManpowerDetails>();
    public DbSet<DailyProgress> DailyProgresses => Set<DailyProgress>();
    public DbSet<DailyProgressDetail> DailyProgressDetails => Set<DailyProgressDetail>();
    public DbSet<DailyProgressPhoto> DailyProgressPhotos => Set<DailyProgressPhoto>();
    public DbSet<DailyLabor> DailyLabors => Set<DailyLabor>();
    public DbSet<DailyLaborDetail> DailyLaborDetails => Set<DailyLaborDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxEvent>(b =>
        {
            b.ToTable("outbox_events");
            b.HasKey(x => x.Id);
            b.Property(x => x.EventType).IsRequired();
            b.Property(x => x.PayloadJson).IsRequired();
            b.Property(x => x.OccurredAt).IsRequired();
        });

        modelBuilder.Entity<Notification>(b =>
        {
            b.ToTable("notifications");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Module).IsRequired();
            b.Property(x => x.EventType).IsRequired();
        });

        modelBuilder.Entity<NotificationDeliveryLog>(b =>
        {
            b.ToTable("notification_delivery_logs");
            b.HasKey(x => x.OutboxEventId);
        });

        modelBuilder.Entity<AuditEvent>(b =>
        {
            b.ToTable("audit_events");
            b.HasKey(x => x.Id);
            b.Property(x => x.Module).IsRequired(false);
            b.Property(x => x.EventType).IsRequired();
            b.Property(x => x.OccurredAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

