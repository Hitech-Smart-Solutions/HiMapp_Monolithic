using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application;

public interface IPMDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public DbSet<DynamicsAssetType> DynamicsAssetTypes { get; set; }
    public DbSet<DynamicsAssetMasterIntegration> DynamicsAssetMasterIntegrations { get; set; }
}
