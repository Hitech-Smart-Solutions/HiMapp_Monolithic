using Microsoft.EntityFrameworkCore;
using Himapp.Workflow.Application;
using Himapp.Workflow.Domain.Entities;

namespace Himapp.Workflow.Infrastructure;

public sealed class WorkflowDbContext : DbContext, IWorkflowDbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    public DbSet<CentralApprovalWorkflow> ApprovalWorkflow { get; set; } = null!;
    public DbSet<CentralApprovalWorkflowProjectDetails> ApprovalWorkflowProjectDetails { get; set; } = null!;
    public DbSet<CentralApprovalWorkflowRoleDetails> ApprovalWorkflowRoleDetails { get; set; } = null!;

    public DbSet<CentralUserRoleMapping> CentralUserRoleMapping { get; set; } = null!;
    public DbSet<CentralUserRoleMappingDetails> CentralUserRoleMappingDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);

        // ============================================================
        // CentralApprovalWorkflow
        // ============================================================

        modelBuilder.Entity<CentralApprovalWorkflow>(entity =>
        {
            entity.ToTable("CentralApprovalWorkflow");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ApprovalWorkflowCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.ApprovalWorkflowDate);

            entity.Property(x => x.ProgramID)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);
        });

        // ============================================================
        // CentralApprovalWorkflowProjectDetails
        // ============================================================

        modelBuilder.Entity<CentralApprovalWorkflowProjectDetails>(entity =>
        {
            entity.ToTable("CentralApprovalWorkflowProjectDetails");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ApprovalWorkflowID)
                .IsRequired();

            entity.Property(x => x.ProjectID)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.ApprovalWorkflow)
                .WithMany(x => x.ApprovalWorkflowProjectDetails)
                .HasForeignKey(x => x.ApprovalWorkflowID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================================
        // CentralApprovalWorkflowRoleDetails
        // ============================================================

        modelBuilder.Entity<CentralApprovalWorkflowRoleDetails>(entity =>
        {
            entity.ToTable("CentralApprovalWorkflowRoleDetails");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ApprovalWorkflowID)
                .IsRequired();

            entity.Property(x => x.RoleID)
                .IsRequired();

            entity.Property(x => x.Priority);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.ApprovalWorkflow)
                .WithMany(x => x.ApprovalWorkflowRoleDetails)
                .HasForeignKey(x => x.ApprovalWorkflowID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================================
        // CentralUserRoleMapping
        // ============================================================

        modelBuilder.Entity<CentralUserRoleMapping>(entity =>
        {
            entity.ToTable("CentralUserRoleMapping");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.RoleCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.RoleName)
                .HasMaxLength(500);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);
        });

        // ============================================================
        // CentralUserRoleMappingDetails
        // ============================================================

        modelBuilder.Entity<CentralUserRoleMappingDetails>(entity =>
        {
            entity.ToTable("CentralUserRoleMappingDetails");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            entity.Property(x => x.UniqueID)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.CentralRoleMappingID)
                .IsRequired();

            entity.Property(x => x.UserID)
                .IsRequired();

            entity.Property(x => x.ProjectID)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.CentralUserRoleMapping)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.CentralRoleMappingID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
