using Microsoft.EntityFrameworkCore;
using Himapp.Safety.Domain.Induction;
using Himapp.Safety.Contracts;

namespace Himapp.Safety.Infrastructure;

public sealed class SafetyDbContext : DbContext, ISafetyDbContext
{
    public SafetyDbContext(DbContextOptions<SafetyDbContext> options) : base(options)
    {
    }

    // DbSets for safety domain entities
    public DbSet<InductionSession> InductionSessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InductionSession>(b =>
        {
            b.ToTable("induction_sessions");
            b.HasKey(x => x.Id);
            b.Property(x => x.TopicSet).IsRequired().HasMaxLength(500);
            b.Property(x => x.SessionDate).IsRequired();
            b.Property(x => x.ProjectId).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.RowVersion).IsRowVersion();
            b.Ignore(x => x.AttendeeLabourIds);
        });
    }
}

