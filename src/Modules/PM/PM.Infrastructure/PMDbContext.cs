using Microsoft.EntityFrameworkCore;
using Himapp.PM.Application;
using Himapp.PM.Domain.Entities;

namespace Himapp.PM.Infrastructure;

public sealed class PMDbContext : DbContext, IPMDbContext
{
    public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
    {
    }
    public DbSet<DynamicsEquipmentCategory> DynamicsEquipmentCategory { get; set; } = null!;
    public DbSet<DynamicsEquipmentMaster> DynamicsEquipmentMaster { get; set; } = null!;

    // DbSets for PM domain entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PAndM"); // your schema name
        base.OnModelCreating(modelBuilder);
    }
}

