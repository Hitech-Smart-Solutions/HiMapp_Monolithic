using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class PlanningDetailConfiguration : IEntityTypeConfiguration<PlanningDetail>
    {
        public void Configure(EntityTypeBuilder<PlanningDetail> builder)
        {
            builder.ToTable("PlanningDetails", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.PlanningID)
                .HasColumnName("PlanningID")
                .IsRequired();

            builder.Property(x => x.AreaID)
                .HasColumnName("AreaID")
                .IsRequired();

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID")
                .IsRequired();

            builder.Property(x => x.TargetQuantity)
                .HasColumnName("TargetQuantity")
                .IsRequired();

            builder.Property(x => x.UOMID)
                .HasColumnName("UOMID")
                .IsRequired();

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

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

            // PlanningDetail -> Planning
            builder.HasOne(x => x.Planning)
                .WithMany(x => x.PlanningDetail)
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
