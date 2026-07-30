using Microsoft.EntityFrameworkCore;

namespace Himapp.Safety.Contracts;

public interface ISafetyDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
