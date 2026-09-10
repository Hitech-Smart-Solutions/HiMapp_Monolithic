using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class ManpowerDetailConfiguration : IEntityTypeConfiguration<ManpowerDetail>
    {
        public void Configure(EntityTypeBuilder<ManpowerDetail> builder)
        {
            builder.ToTable("ManpowerDetails", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ManpowerID)
                .HasColumnName("ManpowerID")
                .IsRequired();

            builder.Property(x => x.ContractorID)
                .HasColumnName("ContractorID")
                .IsRequired();

            builder.Property(x => x.ActivityID)
                .HasColumnName("ActivityID")
                .IsRequired();

            builder.Property(x => x.SkilledCount)
                .HasColumnName("SkilledCount")
                .IsRequired();

            builder.Property(x => x.UnskilledCount)
                .HasColumnName("UnskilledCount")
                .IsRequired();

            builder.Property(x => x.OtherCount)
                .HasColumnName("OtherCount")
                .IsRequired();

            builder.Property(x => x.IsDepartment)
                .HasColumnName("IsDepartment");

            builder.Property(x => x.TotalCount)
                .HasColumnName("TotalCount");

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

            // Relationship
            builder.HasOne(x => x.Manpower)
                .WithMany(x => x.ManpowerDetail)
                .HasForeignKey(x => x.ManpowerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore BaseEntity helper properties
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);

            // ManpowerDetail.Manpower is navigation only
            // and does not need any additional configuration.
        }
    }
}
