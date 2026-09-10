using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class PlanningDocumentDetailConfiguration : IEntityTypeConfiguration<PlanningDocumentDetail>
    {
        public void Configure(EntityTypeBuilder<PlanningDocumentDetail> builder)
        {
            builder.ToTable("PlanningDocumentDetail", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.PlanningID)
                .HasColumnName("PlanningID")
                .IsRequired();

            builder.Property(x => x.DocumentName)
                .HasColumnName("DocumentName")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.FileName)
                .HasColumnName("FileName")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.FilePath)
                .HasColumnName("FilePath")
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.FileExtension)
                .HasColumnName("FileExtension")
                .HasMaxLength(100);

            builder.Property(x => x.ContentType)
                .HasColumnName("ContentType")
                .HasMaxLength(200);

            builder.Property(x => x.FileSize)
                .HasColumnName("FileSize")
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

            // PlanningDocumentDetail -> Planning
            builder.HasOne(x => x.Planning)
                .WithMany(x => x.PlanningDocumentDetail)
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
