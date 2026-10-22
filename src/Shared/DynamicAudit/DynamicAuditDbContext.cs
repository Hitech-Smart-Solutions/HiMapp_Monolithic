using Himapp.DynamicAudit.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.DynamicAudit;

public sealed class DynamicAuditDbContext : DbContext
{
    public DynamicAuditDbContext(DbContextOptions<DynamicAuditDbContext> options) : base(options) { }

    public DbSet<DynamicAuditLog> DynamicAuditLogs => Set<DynamicAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DynamicAuditLog>(b =>
        {
            b.ToTable("DynamicAuditLog");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd().UseIdentityColumn();
            b.Property(x => x.TraceId).HasMaxLength(100).IsRequired();
            b.Property(x => x.UserName).HasMaxLength(200);
            b.Property(x => x.RequestName).HasMaxLength(300).IsRequired();
            b.Property(x => x.Status).HasMaxLength(20).IsRequired();
            b.Property(x => x.ErrorMessage).HasMaxLength(2000);

            b.HasIndex(x => x.UserId).HasDatabaseName("IX_DynamicAuditLog_UserId");
            b.HasIndex(x => x.RequestName).HasDatabaseName("IX_DynamicAuditLog_RequestName");
            b.HasIndex(x => x.Timestamp).HasDatabaseName("IX_DynamicAuditLog_Timestamp");
            b.HasIndex(x => x.Status).HasDatabaseName("IX_DynamicAuditLog_Status");
        });
    }
}
