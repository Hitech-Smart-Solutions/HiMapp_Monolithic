using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Configurations
{
    internal sealed class ManpowerConfiguration : IEntityTypeConfiguration<Manpower>
    {
        public void Configure(EntityTypeBuilder<Manpower> builder)
        {
            builder.ToTable("Manpowers", "execution");

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasColumnName("ID");

            builder.Property(x => x.UniqueID)
                .HasColumnName("UniqueID")
                .IsRequired();

            builder.Property(x => x.ProjectID)
                .HasColumnName("ProjectID")
                .IsRequired();

            builder.Property(x => x.SectionID)
                .HasColumnName("SectionID")
                .IsRequired();

            builder.Property(x => x.EntryDate)
                .HasColumnName("EntryDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .HasMaxLength(1000);

            builder.Property(x => x.StateID)
                .HasColumnName("StateID")
                .HasColumnType("smallint");

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

            // Manpower -> ManpowerDetail
            builder.HasMany(x => x.ManpowerDetail)
                .WithOne(x => x.Manpower)
                .HasForeignKey(x => x.ManpowerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore BaseEntity helper properties
            builder.Ignore(x => x.Id);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.ModifiedAt);
            builder.Ignore(x => x.ModifiedBy);
        }
    }
}
