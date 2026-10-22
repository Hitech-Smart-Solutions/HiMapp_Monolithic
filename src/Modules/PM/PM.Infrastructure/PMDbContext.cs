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
    public DbSet<DynamicsSyncLog> DynamicsSyncLog { get; set; } = null!;

    // DbSets for PM domain entities
    public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
    public DbSet<EquipmentDocument> EquipmentDocuments { get; set; } = null!;
    public DbSet<WorkDescription> WorkDescriptions { get; set; } = null!;
    public DbSet<EquipmentSubPart> EquipmentSubParts { get; set; } = null!;
    public DbSet<EquipmentChecklistPoint> EquipmentChecklistPoints { get; set; } = null!;
    public DbSet<EquipmentBreakdownReasons> EquipmentBreakdownReasons { get; set; } = null!;
    public DbSet<SpecificationMaster> SpecificationMasters { get; set; } = null!;
    public DbSet<PreventiveMaintenancePoint> PreventiveMaintenancePoints { get; set; } = null!;
    public DbSet<MeterReadingType> MeterReadingTypes { get; set; } = null!;
    public DbSet<EquipmentPreventiveMaintenancePoints> EquipmentPreventiveMaintenancePoints { get; set; } = null!;

    public DbSet<EquipmentMeterEpochDetails> EquipmentMeterEpochDetails { get; set; } = null!;
    public DbSet<EquipmentMeterCumulativeDetail> EquipmentMeterCumulativeDetails { get; set; } = null!;

    public DbSet<SparePartCatalogue> SparePartCatalogue { get; set; } = null!;

    // Equipment category <-> master link tables (manual mapping; Remarks + IsActive on each)
    public DbSet<EquipmentCategorySubPart> EquipmentCategorySubParts { get; set; } = null!;
    public DbSet<EquipmentCategoryWorkDescription> EquipmentCategoryWorkDescriptions { get; set; } = null!;
    public DbSet<EquipmentCategoryBreakdownReason> EquipmentCategoryBreakdownReasons { get; set; } = null!;
    public DbSet<EquipmentCategoryPMPoint> EquipmentCategoryPMPoints { get; set; } = null!;
    public DbSet<EquipmentCategoryChecklistPoint> EquipmentCategoryChecklistPoints { get; set; } = null!;
    public DbSet<EquipmentCategorySpecification> EquipmentCategorySpecifications { get; set; } = null!;

    /// <summary>Shared mapping of one category-to-master link table: keys, foreign keys, one row per (category, master).</summary>
    private static void ConfigureCategoryLink<TLink, TMaster>(ModelBuilder modelBuilder, string tableName, string masterIdProperty)
        where TLink : EquipmentCategoryLinkBase
        where TMaster : class
    {
        modelBuilder.Entity<TLink>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsDeleted).IsRequired();

            entity.HasOne<DynamicsEquipmentCategory>()
                  .WithMany()
                  .HasForeignKey(e => e.EquipmentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TMaster>()
                  .WithMany()
                  .HasForeignKey(masterIdProperty)
                  .OnDelete(DeleteBehavior.Restrict);

            // A master can be linked to a category only once; removing a link only sets IsDeleted.
            entity.HasIndex(nameof(EquipmentCategoryLinkBase.EquipmentCategoryId), masterIdProperty).IsUnique();
            entity.HasIndex(masterIdProperty);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PAndM"); // your schema name
        base.OnModelCreating(modelBuilder);

        ConfigureCategoryLink<EquipmentCategorySubPart, EquipmentSubPart>(modelBuilder, "EquipmentCategorySubParts", nameof(EquipmentCategorySubPart.EquipmentSubPartId));
        ConfigureCategoryLink<EquipmentCategoryWorkDescription, WorkDescription>(modelBuilder, "EquipmentCategoryWorkDescriptions", nameof(EquipmentCategoryWorkDescription.WorkDescriptionId));
        ConfigureCategoryLink<EquipmentCategoryBreakdownReason, EquipmentBreakdownReasons>(modelBuilder, "EquipmentCategoryBreakdownReasons", nameof(EquipmentCategoryBreakdownReason.EquipmentBreakdownReasonId));
        ConfigureCategoryLink<EquipmentCategoryPMPoint, PreventiveMaintenancePoint>(modelBuilder, "EquipmentCategoryPMPoints", nameof(EquipmentCategoryPMPoint.PreventiveMaintenancePointId));
        ConfigureCategoryLink<EquipmentCategoryChecklistPoint, EquipmentChecklistPoint>(modelBuilder, "EquipmentCategoryChecklistPoints", nameof(EquipmentCategoryChecklistPoint.EquipmentChecklistPointId));
        ConfigureCategoryLink<EquipmentCategorySpecification, SpecificationMaster>(modelBuilder, "EquipmentCategorySpecifications", nameof(EquipmentCategorySpecification.SpecificationMasterId));

        // ============================================================
        // DynamicsSyncLog (one row per Dynamics sync / reset run)
        // ============================================================
        modelBuilder.Entity<DynamicsSyncLog>(entity =>
        {
            entity.ToTable("DynamicsSyncLog");

            entity.HasKey(x => x.ID);

            entity.Property(x => x.ID)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.EntityName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SyncMode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.TriggerSource).HasMaxLength(20).IsRequired();
            entity.Property(x => x.JobId).HasMaxLength(100);
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();

            // Used by the "is a sync already running?" check.
            entity.HasIndex(x => new { x.EntityName, x.Status });
        });

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

        modelBuilder.Entity<EquipmentDocument>(entity =>
        {
            entity.ToTable("EquipmentDocuments");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.DocumentTypeID)
                  .IsRequired();

            entity.Property(e => e.DocumentName)
                  .IsRequired();

            entity.Property(e => e.StartDate)
                  .IsRequired();

            entity.Property(e => e.EndDate)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true)
                  .IsRequired(false);

            entity.HasOne<DocumentType>()
                  .WithMany()
                  .HasForeignKey(e => e.DocumentTypeID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.DocumentTypeID);
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

        modelBuilder.Entity<SpecificationMaster>(entity =>
        {
            entity.ToTable("SpecificationMaster");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.SpecificationName)
                  .IsRequired();

            entity.Property(e => e.Type)
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
                .HasColumnType("timestamp with time zone");

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