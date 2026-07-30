using Microsoft.EntityFrameworkCore;
using Himapp.PM.Application;

namespace Himapp.PM.Infrastructure;

public sealed class PMDbContext : DbContext, IPMDbContext
{
    public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
    {
    }

    // DbSets for PM domain entities
}

