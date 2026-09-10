using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class DailyDepartmentalLabourSlipConfiguration
    : IEntityTypeConfiguration<DailyDepartmentalLabourSlip>
    {
        public void Configure(EntityTypeBuilder<DailyDepartmentalLabourSlip> builder)
        {
            builder.ToTable("DailyDepartmentalLabourSlips", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID");

            builder.Property(x => x.DDLSlipCode)
                .HasColumnName("DDLSlipCode")
                .HasMaxLength(50);

            builder.Property(x => x.SlipDate)
                .HasColumnName("SlipDate")
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.IssueNumber)
                .HasColumnName("IssueNumber")
                .HasMaxLength(100);

            builder.Property(x => x.PartyID)
                .HasColumnName("PartyID");

            builder.Property(x => x.IsNewParty)
                .HasColumnName("IsNewParty");

            builder.Property(x => x.NewParty)
                .HasColumnName("NewParty")
                .HasMaxLength(500);

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.StatusID)
                .HasColumnName("StatusID");

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive");

            builder.Property(x => x.DocumentName)
                .HasColumnName("DocumentName");

            builder.Property(x => x.DocumentContentType)
                .HasColumnName("DocumentContentType");

            builder.Property(x => x.DocumentPath)
                .HasColumnName("DocumentPath");

            builder.Property(x => x.IsDisapproved)
                .HasColumnName("IsDisapproved");

            builder.Property(x => x.TotalWrkMins)
                .HasColumnName("TotalWrkMins");

            builder.Property(x => x.DPRSlipIssueID)
                .HasColumnName("DPRSlipIssueID");

            builder.Property(x => x.TotalDPRManpower)
                .HasColumnName("TotalDPRManpower");

            builder.Property(x => x.Skilled)
                .HasColumnName("Skilled");

            builder.Property(x => x.UnSkilled)
                .HasColumnName("UnSkilled");

            builder.Property(x => x.Mat)
                .HasColumnName("Mat");

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
            builder.HasMany(x => x.DailyDepartmentalLabourSlipDetails)
                .WithOne(x => x.DailyDepartmentalLabourSlip)
                .HasForeignKey(x => x.DDLSlipID)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity properties that are not DB columns
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
