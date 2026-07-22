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

        builder.Property(x => x.UniqueID).IsRequired();
        builder.Property(x => x.DLRDate).HasColumnName("ReportDate");
        builder.Property(x => x.Remarks).HasMaxLength(1000);
        builder.Property(x => x.StateID).HasColumnType("smallint");

        // One-to-many: DailyLabor -> DailyLaborDetail
        // We choose Cascade delete here to ensure child details are removed when a header
        // is deleted at the database level. Application layer performs soft-delete for headers,
        // so cascade is a safety for physical deletes only in exceptional maintenance flows.
        builder.HasMany(d => d.DailyLaborDetail)
               .WithOne(d => d.DailyLabor)
               .HasForeignKey(d => d.DailyLabourID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
