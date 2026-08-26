using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class DailyProgressConfiguration
    : IEntityTypeConfiguration<DailyProgress>
    {
        public void Configure(EntityTypeBuilder<DailyProgress> builder)
        {
            builder.ToTable("DailyProgress", "execution");

            // Primary Key
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            // Main fields
            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID")
                .IsRequired();

            builder.Property(x => x.DPRCode)
                .HasColumnName("DPRCode")
                .HasMaxLength(50);

            builder.Property(x => x.ReportDate)
                .HasColumnName("ReportDate")
                .HasColumnType("date");

            builder.Property(x => x.NextDayPlan)
                .HasColumnName("NextDayPlan")
                .HasMaxLength(2000);

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.TotalAmount)
                .HasColumnName("TotalAmount")
                .HasColumnType("numeric");

            builder.Property(x => x.StatusID)
                .HasColumnName("StatusID");

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

            // Parent -> Details
            builder.HasMany(x => x.DailyProgressDetail)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // Parent -> Hindrances
            builder.HasMany(x => x.DailyProgressHindrance)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // Parent -> Photos
            builder.HasMany(x => x.DailyProgressPhoto)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity properties that do not exist as DB columns
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
