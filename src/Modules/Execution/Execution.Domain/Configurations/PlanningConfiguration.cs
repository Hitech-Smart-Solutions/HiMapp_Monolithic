using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class PlanningConfiguration : IEntityTypeConfiguration<Planning>
    {
        public void Configure(EntityTypeBuilder<Planning> builder)
        {
            builder.ToTable("Plannings", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID")
                .IsRequired();

            builder.Property(x => x.AreaID)
                .HasColumnName("AreaID")
                .IsRequired();

            builder.Property(x => x.PlanTypeID)
                .HasColumnName("PlanTypeID")
                .IsRequired();

            builder.Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .HasColumnType("date");

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.StatusID)
                .HasColumnName("StatusID")
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

            // Planning -> PlanningDetail
            builder.HasMany(x => x.PlanningDetail)
                .WithOne(x => x.Planning)
                .HasForeignKey(x => x.PlanningID)
                .OnDelete(DeleteBehavior.Cascade);

            // Planning -> PlanningDocumentDetail
            builder.HasMany(x => x.PlanningDocumentDetail)
                .WithOne(x => x.Planning)
                .HasForeignKey(x => x.PlanningID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore BaseEntity helper properties
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
