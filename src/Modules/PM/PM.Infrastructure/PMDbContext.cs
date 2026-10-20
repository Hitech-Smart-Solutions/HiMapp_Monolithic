using Microsoft.EntityFrameworkCore;
using Himapp.PM.Application;
using Himapp.PM.Domain.Entities;

namespace Himapp.PM.Infrastructure;

public sealed class PMDbContext : DbContext, IPMDbContext
{
    public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
    {
    }
    public DbSet<DynamicsAssetType> DynamicsAssetTypes { get; set; } = null!;
    public DbSet<DynamicsAssetMasterIntegration> DynamicsAssetMasterIntegrations { get; set; } = null!;

    // DbSets for PM domain entities
}

