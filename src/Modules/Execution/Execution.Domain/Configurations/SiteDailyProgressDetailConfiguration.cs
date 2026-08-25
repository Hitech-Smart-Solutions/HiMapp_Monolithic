using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class SiteDailyProgressDetailConfiguration : IEntityTypeConfiguration<SiteDailyProgressDetail>
    {
        public void Configure(EntityTypeBuilder<SiteDailyProgressDetail> builder)
        {
            builder.ToTable("SiteDailyProgressDetail", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.SiteDailyProgressID)
                .HasColumnName("SiteDailyProgressID")
                .IsRequired();

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID")
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.UOMID)
                .HasColumnName("UOMID")
                .IsRequired();

            builder.Property(x => x.Rate)
                .HasColumnName("Rate")
                .HasColumnType("numeric")
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("Amount")
                .HasColumnType("numeric");

            builder.Property(x => x.PlanQuantity)
                .HasColumnName("PlanQuantity")
                .HasColumnType("numeric");

            builder.Property(x => x.Variance)
                .HasColumnName("Variance")
                .HasColumnType("numeric");

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

            // Relationship with DailyProgress
            builder.HasOne(x => x.DailyProgress)
                .WithMany(x => x.SiteDailyProgressDetail)
                .HasForeignKey(x => x.SiteDailyProgressID)
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
