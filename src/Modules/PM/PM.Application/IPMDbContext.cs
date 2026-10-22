using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Himapp.PM.Application;

public interface IPMDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public DbSet<DynamicsEquipmentCategory> DynamicsEquipmentCategory { get; set; }
    public DbSet<DynamicsEquipmentMaster> DynamicsEquipmentMaster { get; set; }
}
