using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Himapp.Execution.Domain.Configurations;

internal sealed class LockRequestDetailsConfiguration : IEntityTypeConfiguration<LockRequestDetails>
{
    public void Configure(EntityTypeBuilder<LockRequestDetails> builder)
    {
        builder.ToTable("LockRequestDetails", "execution");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.ID)
            .HasColumnName("ID");

        builder.Property(x => x.UniqueID)
            .HasColumnName("UniqueID")
            .IsRequired();

        builder.Property(x => x.LockRequestID)
            .HasColumnName("LockRequestID");

        builder.Property(x => x.OpenDate)
            .HasColumnName("OpenDate");

        builder.Property(x => x.Reason)
            .HasColumnName("Reason")
            .HasMaxLength(1000);

        builder.Property(x => x.Duration)
            .HasColumnName("Duration");

        builder.Property(x => x.Counts)
            .HasColumnName("Counts");

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

        // Relationship
        builder.HasOne(x => x.LockRequest)
            .WithMany(x => x.LockRequestDetails)
            .HasForeignKey(x => x.LockRequestID)
            .OnDelete(DeleteBehavior.Cascade);

        // BaseEntity properties not present as DB columns
        builder.Ignore(x => x.Id);
        builder.Ignore(x => x.CreatedAt);
        builder.Ignore(x => x.ModifiedAt);
        builder.Ignore(x => x.ModifiedBy);
    }
}