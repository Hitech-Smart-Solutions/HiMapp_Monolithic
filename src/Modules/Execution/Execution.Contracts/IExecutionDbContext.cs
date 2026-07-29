using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Contracts;

public interface IExecutionDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
