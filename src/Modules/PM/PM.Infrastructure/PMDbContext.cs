using Himapp.PM.Application;
using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Infrastructure;

public sealed class PMDbContext : DbContext, IPMDbContext
{
    public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
    {
    }
    public DbSet<DynamicsEquipmentCategory> DynamicsEquipmentCategory { get; set; } = null!;
    public DbSet<DynamicsEquipmentMaster> DynamicsEquipmentMaster { get; set; } = null!;

    // DbSets for PM domain entities
    public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
    public DbSet<WorkDescription> WorkDescriptions { get; set; } = null!;
    public DbSet<EquipmentSubPart> EquipmentSubParts { get; set; } = null!;
    public DbSet<EquipmentChecklistPoint> EquipmentChecklistPoints { get; set; } = null!;
    public DbSet<EquipmentBreakdownReasons> EquipmentBreakdownReasons { get; set; } = null!;
    public DbSet<PreventiveMaintenancePoint> PreventiveMaintenancePoints { get; set; } = null!;
    public DbSet<MeterReadingType> MeterReadingTypes { get; set; } = null!;
    public DbSet<EquipmentPreventiveMaintenancePoints> EquipmentPreventiveMaintenancePoints { get; set; } = null!;

    public DbSet<EquipmentMeterEpochDetails> EquipmentMeterEpochDetails { get; set; } = null!;
    public DbSet<EquipmentMeterCumulativeDetail> EquipmentMeterCumulativeDetails { get; set; } = null!;

    public DbSet<SparePartCatalogue> SparePartCatalogue { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PAndM"); // your schema name
        base.OnModelCreating(modelBuilder);

        // ============================================================
        // PreventiveMaintenancePoint
        // ============================================================
        modelBuilder.Entity<PreventiveMaintenancePoint>(entity =>
        {
            entity.ToTable("PreventiveMaintenancePoint");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.MaintenancePointName)
                .HasColumnName("MaintenancePointName")
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            entity.Property(x => x.StatusID)
                .HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);
        });

        // ============================================================
        // MeterReadingType
        // ============================================================
        modelBuilder.Entity<MeterReadingType>(entity =>
        {
            entity.ToTable("MeterReadingType");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ReadingTypeName)
                .HasColumnName("ReadingTypeName")
                .HasMaxLength(50)
                .IsUnicode(false)                   // varchar, not nvarchar
                .IsRequired();

            entity.Property(x => x.Description)
                .HasColumnName("Description")
                .IsUnicode(false)                   // varchar(MAX)
                .IsRequired(false);

            entity.Property(x => x.StatusID)
                .HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

        });

        modelBuilder.Entity<SparePartCatalogue>(entity =>
        {
            entity.ToTable("SparePartCatalogue");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.EquipmentcategoryID)
                .HasColumnName("EquipmentcategoryID");

            entity.Property(x => x.BrandName)
                .HasColumnName("BrandName");

            entity.Property(x => x.ModelName)
                .HasColumnName("ModelName");

            entity.Property(x => x.StatusID)
                .HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

        });

        // ============================================================
        // EquipmentPreventiveMaintenancePoints
        // ============================================================
        modelBuilder.Entity<EquipmentPreventiveMaintenancePoints>(entity =>
        {
            entity.ToTable("EquipmentPreventiveMaintenancePoints");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.EquipmentID).HasColumnName("EquipmentID");
            entity.Property(x => x.MaintenancePointID).HasColumnName("MaintenancePointID");
            entity.Property(x => x.ItemID).HasColumnName("ItemID");
            entity.Property(x => x.ReadingTypeID).HasColumnName("ReadingTypeID");

            entity.Property(x => x.IntervalValue)
                .HasColumnName("IntervalValue")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.TolerenceValue)
                .HasColumnName("TolerenceValue")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Months).HasColumnName("Months");

            entity.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .IsUnicode(false)
                .IsRequired(false);

            entity.Property(x => x.StatusID)
                .HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

            entity.HasIndex(x => x.EquipmentID)
                .HasDatabaseName("IX_EquipmentPreventiveMaintenancePoints_EquipmentID");
        });


        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentTypes");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WorkDescription>(entity =>
        {
            entity.ToTable("WorkDescriptions");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.WorkDescriptionName)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        modelBuilder.Entity<EquipmentSubPart>(entity =>
        {
            entity.ToTable("EquipmentSubParts");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.EquipmentSubPartName)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        modelBuilder.Entity<EquipmentChecklistPoint>(entity =>
        {
            entity.ToTable("EquipmentChecklistPoints");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Description)
                  .IsRequired();

            entity.Property(e => e.Type)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        modelBuilder.Entity<EquipmentBreakdownReasons>(entity =>
        {
            entity.ToTable("EquipmentBreakdownReasons");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.BreakdownReason)
                  .IsRequired();

            entity.Property(e => e.Type)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });


        // ============================================================
        // EquipmentMeterEpochDetails
        // ============================================================
        modelBuilder.Entity<EquipmentMeterEpochDetails>(entity =>
        {
            entity.ToTable("EquipmentMeterEpochDetails");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.EquipmentID).HasColumnName("EquipmentID");
            entity.Property(x => x.MeterReadingID).HasColumnName("MeterReadingID");
            entity.Property(x => x.ReasonID).HasColumnName("ReasonID");

            entity.Property(x => x.OpenningDate)
                .HasColumnName("OpenningDate")
                .HasColumnType("datetime");

            entity.Property(x => x.ResetReadingAmount)
                .HasColumnName("ResetReadingAmount")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.OpeningReadingAmount)
                .HasColumnName("OpeningReadingAmount")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Remarks)
                .HasColumnName("Remarks")
                .IsUnicode(false)
                .IsRequired(false);

            entity.Property(x => x.StatusID).HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

            entity.HasIndex(x => x.EquipmentID)
                .HasDatabaseName("IX_EquipmentMeterEpochDetails_EquipmentID");

        });

        // ============================================================
        // EquipmentMeterCumulativeDetail
        // ============================================================
        modelBuilder.Entity<EquipmentMeterCumulativeDetail>(entity =>
        {
            entity.ToTable("EquipmentMeterCumulativeDetail");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.EquipmentID).HasColumnName("EquipmentID");

            entity.Property(x => x.FinalReadingAmount)
                .HasColumnName("FinalReadingAmount")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.ActualOpeningReadingAmount)
                .HasColumnName("ActualOpeningReadingAmount")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.StatusID).HasColumnName("StatusID");

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

            entity.HasIndex(x => x.EquipmentID)
                .HasDatabaseName("IX_EquipmentMeterCumulativeDetail_EquipmentID");

        });


    }
}