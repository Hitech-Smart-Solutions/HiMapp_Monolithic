using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Himapp.Execution.Domain.Entities;

namespace Himapp.Execution.Domain.Configurations;

internal sealed class DailyLaborConfiguration : IEntityTypeConfiguration<DailyLabor>
{
    public void Configure(EntityTypeBuilder<DailyLabor> builder)
    {
        builder.ToTable("DailyLabor");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.ID)
            .HasColumnName("ID");

        builder.Property(x => x.UniqueID)
            .HasColumnName("UniqueID")
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .HasColumnName("CreatedDate");

        builder.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy");

        builder.Property(x => x.LastModifiedDate)
            .HasColumnName("LastModifiedDate");

        builder.Property(x => x.LastModifiedBy)
            .HasColumnName("LastModifiedBy");

        builder.Property(x => x.DLRDate)
            .HasColumnName("ReportDate");

        builder.Property(x => x.DLRCode)
            .HasMaxLength(50);

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);

        builder.Property(x => x.StateID)
            .HasColumnType("smallint");

        builder.HasIndex(x => x.DLRCode)
            .IsUnique();

        builder.HasMany(d => d.DailyLaborDetail)
            .WithOne(d => d.DailyLabor)
            .HasForeignKey(d => d.DailyLabourID)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(x => x.Id);
        builder.Ignore(x => x.CreatedAt);
        builder.Ignore(x => x.ModifiedAt);
        builder.Ignore(x => x.ModifiedBy);
    }
}
