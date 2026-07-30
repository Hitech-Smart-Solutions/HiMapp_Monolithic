using Microsoft.EntityFrameworkCore;

namespace Himapp.Store.Application;

public interface IStoreDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
