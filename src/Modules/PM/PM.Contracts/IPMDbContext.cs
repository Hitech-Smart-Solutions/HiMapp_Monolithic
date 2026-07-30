using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Contracts;

public interface IPMDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
