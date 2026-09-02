using Microsoft.EntityFrameworkCore;
using Himapp.Store.Domain.GatePass;
using Himapp.Store.Application;

namespace Himapp.Store.Infrastructure;

public sealed class StoreDbContext : DbContext, IStoreDbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    // DbSets for store domain entities
    public DbSet<GatePass> GatePasses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GatePass>(b =>
        {
            b.ToTable("gate_passes");
            b.HasKey(x => x.Id);
            b.Property(x => x.GatePassNo).IsRequired().HasMaxLength(50);
            b.Property(x => x.Path).HasMaxLength(10);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.BackdatedReason).HasMaxLength(500);
            b.Property(x => x.CancelReason).HasMaxLength(500);
            b.Property(x => x.ProjectId).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Ignore(x => x.Lines);
        });
    }
}

