using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class ProjectActivityConfiguration : IEntityTypeConfiguration<ProjectActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectActivity> builder)
        {
            builder.ToTable("ProjectActivities", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID")
                .IsRequired();

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID")
                .IsRequired();

            builder.Property(x => x.RevenueRate)
                .HasColumnName("RevenueRate")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.SkilledLabourRate)
                .HasColumnName("SkilledLabourRate")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.UnSkilledLabourRate)
                .HasColumnName("UnSkilledLabourRate")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.OtherLabourRate)
                .HasColumnName("OtherLabourRate")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.OutputRequired)
                .HasColumnName("OutputRequired")
                .IsRequired();

            builder.Property(x => x.Enabled)
                .HasColumnName("Enabled")
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

            // Ignore BaseEntity helper properties
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
