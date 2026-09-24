using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himapp.Execution.Domain.Configurations;

internal sealed class LockRequestConfiguration : IEntityTypeConfiguration<LockRequest>
{
    public void Configure(EntityTypeBuilder<LockRequest> builder)
    {
        builder.ToTable("LockRequest", "execution");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.ID)
            .HasColumnName("ID");

        builder.Property(x => x.UniqueID)
            .HasColumnName("UniqueID")
            .IsRequired();

        builder.Property(x => x.RequestCode)
            .HasColumnName("RequestCode")
            .HasMaxLength(50);

        builder.Property(x => x.RequestDate)
            .HasColumnName("RequestDate");

        builder.Property(x => x.Remarks)
            .HasColumnName("Remarks")
            .HasMaxLength(1000);

        builder.Property(x => x.CompanyID)
            .HasColumnName("CompanyID");

        builder.Property(x => x.ProjectID)
            .HasColumnName("ProjectID");

        builder.Property(x => x.ProgramID)
            .HasColumnName("ProgramID");

        builder.Property(x => x.StateID)
            .HasColumnName("StateID")
            .HasColumnType("smallint");

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive");

        // Audit fields from BaseEntity
        builder.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy");

        builder.Property(x => x.CreatedDate)
            .HasColumnName("CreatedDate");

        builder.Property(x => x.LastModifiedBy)
            .HasColumnName("LastModifiedBy");

        builder.Property(x => x.LastModifiedDate)
            .HasColumnName("LastModifiedDate");

        // Header -> Details relationship
        builder.HasMany(x => x.LockRequestDetails)
            .WithOne(x => x.LockRequest)
            .HasForeignKey(x => x.LockRequestID)
            .OnDelete(DeleteBehavior.Cascade);

        // BaseEntity properties that are not DB columns
        builder.Ignore(x => x.Id);
        builder.Ignore(x => x.CreatedAt);
        builder.Ignore(x => x.ModifiedAt);
        builder.Ignore(x => x.ModifiedBy);
    }
}