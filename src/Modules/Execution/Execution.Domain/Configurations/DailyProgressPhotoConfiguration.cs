using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class DailyProgressPhotoConfiguration
    : IEntityTypeConfiguration<DailyProgressPhoto>
    {
        public void Configure(EntityTypeBuilder<DailyProgressPhoto> builder)
        {
            builder.ToTable("DailyProgressPhotos", "execution");

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

            builder.Property(x => x.FileName)
                .HasColumnName("FileName")
                .HasMaxLength(500);

            builder.Property(x => x.FileType)
                .HasColumnName("FileType")
                .HasMaxLength(100);

            builder.Property(x => x.FileSize)
                .HasColumnName("FileSize");

            builder.Property(x => x.PhotoUrl)
                .HasColumnName("PhotoUrl")
                .HasMaxLength(1000);

            builder.Property(x => x.Caption)
                .HasColumnName("Caption")
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
                .WithMany(x => x.DailyProgressPhoto)
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
