using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Himapp.Execution.Domain.Entities;

namespace Himapp.Execution.Domain.Configurations;

internal sealed class DailyLaborDetailConfiguration : IEntityTypeConfiguration<DailyLaborDetail>
{
    public void Configure(EntityTypeBuilder<DailyLaborDetail> builder)
    {
        builder.ToTable("DailyLaborDetails");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.UniqueID).IsRequired();
        builder.Property(x => x.Remarks).HasMaxLength(1000);
    }
}
