using Microsoft.EntityFrameworkCore;

namespace Himapp.Admin.Application;

public interface IAdminDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
