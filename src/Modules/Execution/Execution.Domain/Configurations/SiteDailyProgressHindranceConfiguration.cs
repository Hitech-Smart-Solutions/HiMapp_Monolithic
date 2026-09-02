using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class SiteDailyProgressHindranceConfiguration : IEntityTypeConfiguration<SiteDailyProgressHindrance>
    {
        public void Configure(EntityTypeBuilder<SiteDailyProgressHindrance> builder)
        {
            builder.ToTable("SiteDailyProgressHindrance", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.DailyProgressID)
                .HasColumnName("DailyProgressID")
                .IsRequired();

            builder.Property(x => x.Hindrance)
                .HasColumnName("Hindrance")
                .HasMaxLength(2000);

            builder.Property(x => x.AudioUrl)
                .HasColumnName("AudioUrl")
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
                .WithMany(x => x.SiteDailyProgressHindrance)
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
