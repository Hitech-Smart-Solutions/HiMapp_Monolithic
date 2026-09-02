using Microsoft.EntityFrameworkCore;
using Himapp.Admin.Domain.Labour;
using Himapp.Admin.Application;

namespace Himapp.Admin.Infrastructure;

public sealed class AdminDbContext : DbContext, IAdminDbContext
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
    {
    }

    // DbSets for admin domain entities
    public DbSet<Labour> Labours { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Labour>(b =>
        {
            b.ToTable("labours");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.AadhaarMasked).IsRequired().HasMaxLength(12);
            b.Property(x => x.AadhaarHash).IsRequired();
            b.Property(x => x.Pan).HasMaxLength(10);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.DateOfBirth).IsRequired();
            b.Property(x => x.ProjectId).IsRequired();
            b.Property(x => x.ContractorId).IsRequired();
            b.Property(x => x.PhotoFileId).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
        });
    }
}

