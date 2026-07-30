using Microsoft.EntityFrameworkCore;

namespace Himapp.Store.Contracts;

public interface IStoreDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
