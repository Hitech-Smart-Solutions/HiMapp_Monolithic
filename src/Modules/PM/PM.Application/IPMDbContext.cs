using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application;

public interface IPMDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public DbSet<DynamicsEquipmentCategory> DynamicsEquipmentCategory { get; set; }
    public DbSet<DynamicsEquipmentMaster> DynamicsEquipmentMaster { get; set; }
}
