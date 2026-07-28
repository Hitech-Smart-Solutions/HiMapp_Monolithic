using Himapp.Audit.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Audit;

/// <summary>
/// Dedicated DbContext for audit log tables.
/// Using a separate context prevents log writes from contending with
/// transactional business data (as recommended in US-LOG-007).
/// </summary>
public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<TransactionActionHistory> TransactionActionHistories => Set<TransactionActionHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TransactionActionHistory>(b =>
        {
            b.ToTable("TransactionActionHistory");

            b.HasKey(x => x.Id);

            b.Property(x => x.UniqueID)
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired();

            b.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.ActionId)
                .IsRequired();

            b.Property(x => x.ProgramId)
                .IsRequired();

            b.Property(x => x.ProgramRowId)
                .IsRequired();

            b.Property(x => x.ProgramRowCode)
                .HasMaxLength(200);

            b.Property(x => x.Remarks)
                .HasMaxLength(1000);

            b.Property(x => x.IsActive)
                .HasDefaultValue(true);

            b.Property(x => x.CreatedDate)
                .IsRequired();

            b.Property(x => x.LastModifiedDate)
                .IsRequired();

            // Recommended indexes for querying
            b.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_TransactionActionHistory_UserId");

            b.HasIndex(x => x.ActionId)
                .HasDatabaseName("IX_TransactionActionHistory_ActionId");

            b.HasIndex(x => x.ProgramId)
                .HasDatabaseName("IX_TransactionActionHistory_ProgramId");

            b.HasIndex(x => x.CreatedDate)
                .HasDatabaseName("IX_TransactionActionHistory_CreatedDate");

            b.HasIndex(x => new { x.ProgramId, x.ActionId, x.CreatedDate })
                .HasDatabaseName("IX_TransactionActionHistory_Program_Action_Date");
        });
    }
}
