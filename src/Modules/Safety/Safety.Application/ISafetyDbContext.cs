using Microsoft.EntityFrameworkCore;

namespace Himapp.Safety.Application;

public interface ISafetyDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
