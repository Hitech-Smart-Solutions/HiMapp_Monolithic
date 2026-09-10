using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class SiteDailyProgressPhotoConfiguration : IEntityTypeConfiguration<SiteDailyProgressPhoto>
    {
        public void Configure(EntityTypeBuilder<SiteDailyProgressPhoto> builder)
        {
            builder.ToTable("SiteDailyProgressPhoto", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.DailyProgressID)
                .HasColumnName("DailyProgressID")
                .IsRequired();

            builder.Property(x => x.FileName)
                .HasColumnName("FileName")
                .HasMaxLength(255);

            builder.Property(x => x.FileType)
                .HasColumnName("FileType")
                .HasMaxLength(100);

            builder.Property(x => x.FileSize)
                .HasColumnName("FileSize");

            builder.Property(x => x.PhotoUrl)
                .HasColumnName("PhotoUrl")
                .IsRequired();

            builder.Property(x => x.Caption)
                .HasColumnName("Caption")
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
                .WithMany(x => x.SiteDailyProgressPhoto)
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
