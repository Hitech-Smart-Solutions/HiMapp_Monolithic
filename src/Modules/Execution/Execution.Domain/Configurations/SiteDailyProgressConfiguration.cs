using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class SiteDailyProgressConfiguration : IEntityTypeConfiguration<SiteDailyProgress>
    {
        public void Configure(EntityTypeBuilder<SiteDailyProgress> builder)
        {
            builder.ToTable("SiteDailyProgresses", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID")
                .IsRequired();

            builder.Property(x => x.SectionID)
                .HasColumnName("SectionID");

            builder.Property(x => x.ReportDate)
                .HasColumnName("ReportDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.NextDayPlan)
                .HasColumnName("NextDayPlan")
                .HasMaxLength(2000);

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.TotalAmount)
                .HasColumnName("TotalAmount")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .HasColumnName("CreatedDate");

            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.LastModifiedDate)
                .HasColumnName("LastModifiedDate");

            builder.Property(x => x.LastModifiedBy)
                .HasColumnName("LastModifiedBy");

            // Details
            builder.HasMany(x => x.SiteDailyProgressDetail)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.SiteDailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // Photos
            builder.HasMany(x => x.SiteDailyProgressPhoto)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // Hindrances
            builder.HasMany(x => x.SiteDailyProgressHindrance)
                .WithOne(x => x.DailyProgress)
                .HasForeignKey(x => x.DailyProgressID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore BaseEntity helper properties
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
