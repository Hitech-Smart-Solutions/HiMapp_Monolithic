using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Infrastructure;

public sealed class PMDbContext : DbContext
{
    public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
    {
    }

    // DbSets for PM domain entities
}

