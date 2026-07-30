using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application;

public interface IExecutionDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
