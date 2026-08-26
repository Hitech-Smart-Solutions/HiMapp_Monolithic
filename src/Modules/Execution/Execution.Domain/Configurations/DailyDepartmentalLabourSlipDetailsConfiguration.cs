using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class DailyDepartmentalLabourSlipDetailsConfiguration
    : IEntityTypeConfiguration<DailyDepartmentalLabourSlipDetails>
    {
        public void Configure(
            EntityTypeBuilder<DailyDepartmentalLabourSlipDetails> builder)
        {
            builder.ToTable("DailyDepartmentalLabourSlipDetails", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.DDLSlipID)
                .HasColumnName("DDLSlipID");

            builder.Property(x => x.LabourCategoryTypeID)
                .HasColumnName("LabourCategoryTypeID");

            builder.Property(x => x.NumOfLabour)
                .HasColumnName("NumOfLabour");

            builder.Property(x => x.FromTime)
                .HasColumnName("FromTime")
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.TOTime)
                .HasColumnName("TOTime")
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.LunchHour)
                .HasColumnName("LunchHour")
                .HasColumnType("numeric");

            builder.Property(x => x.WorkingHours)
                .HasColumnName("WorkingHours")
                .HasColumnType("numeric");

            builder.Property(x => x.WorkLocationID)
                .HasColumnName("WorkLocationID");

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID");

            builder.Property(x => x.ActivityDetails)
                .HasColumnName("ActivityDetails");

            builder.Property(x => x.UOMID)
                .HasColumnName("UOMID");

            builder.Property(x => x.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("numeric");

            builder.Property(x => x.DebitPartyID)
                .HasColumnName("DebitPartyID");

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.StateID)
                .HasColumnName("StateID")
                .HasColumnType("smallint");

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive");

            builder.Property(x => x.IsLumSumWork)
                .HasColumnName("IsLumSumWork");

            // BaseEntity audit fields
            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.CreatedDate)
                .HasColumnName("CreatedDate");

            builder.Property(x => x.LastModifiedBy)
                .HasColumnName("LastModifiedBy");

            builder.Property(x => x.LastModifiedDate)
                .HasColumnName("LastModifiedDate");

            // Relationship
            builder.HasOne(x => x.DailyDepartmentalLabourSlip)
                .WithMany(x => x.DailyDepartmentalLabourSlipDetails)
                .HasForeignKey(x => x.DDLSlipID)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity properties not present as DB columns
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
