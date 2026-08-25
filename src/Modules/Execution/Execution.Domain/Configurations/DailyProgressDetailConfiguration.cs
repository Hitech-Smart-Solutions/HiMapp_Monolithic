using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class DailyProgressDetailConfiguration
    : IEntityTypeConfiguration<DailyProgressDetail>
    {
        public void Configure(EntityTypeBuilder<DailyProgressDetail> builder)
        {
            builder.ToTable("DailyProgressDetails", "execution");

            // Primary Key
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            // Foreign Key
            builder.Property(x => x.DailyProgressID)
                .HasColumnName("DailyProgressID")
                .IsRequired();

            // Activity
            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID")
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("numeric");

            builder.Property(x => x.UOMID)
                .HasColumnName("UOMID");

            builder.Property(x => x.Rate)
                .HasColumnName("Rate")
                .HasColumnType("numeric");

            // Computed stored column
            builder.Property(x => x.Amount)
                .HasColumnName("Amount")
                .HasColumnType("numeric");

            builder.Property(x => x.PlanQuantity)
                .HasColumnName("PlanQuantity")
                .HasColumnType("numeric");

            // Computed stored column
            builder.Property(x => x.Variance)
                .HasColumnName("Variance")
                .HasColumnType("numeric");

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

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

            // Relationship
            builder.HasOne(x => x.DailyProgress)
                .WithMany(x => x.DailyProgressDetail)
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
