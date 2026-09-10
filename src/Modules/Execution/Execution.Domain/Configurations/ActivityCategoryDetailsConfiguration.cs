using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal class ActivityCategoryDetailsConfiguration : IEntityTypeConfiguration<ActivityCategoryDetails>
    {
        public void Configure(EntityTypeBuilder<ActivityCategoryDetails> builder)
        {
            builder.ToTable("ActivityCategoryDetails");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID");

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID");

            builder.Property(x => x.CategoryTypeID)
                .HasColumnName("CategoryTypeID");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasMaxLength(500);

            builder.Property(x => x.Rate)
                .HasColumnName("Rate");

            builder.Property(x => x.IsActive)
                .HasColumnName("IsActive");

            // BaseEntity audit fields
            builder.Property(x => x.CreatedDate)
                .HasColumnName("CreatedDate");

            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.LastModifiedDate)
                .HasColumnName("LastModifiedDate");

            builder.Property(x => x.LastModifiedBy)
                .HasColumnName("LastModifiedBy");
        }
    }
}
