using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class PM_SparePart_v05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PAndM");

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentTypeName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DynamicsEquipmentCategory",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataAreaId = table.Column<string>(type: "text", nullable: false),
                    OBJECTTYPEID = table.Column<string>(type: "text", nullable: false),
                    ISFUELCONSUMPTIONREQUIRED_ISPL = table.Column<int>(type: "integer", nullable: false),
                    ISLOGBOOKREQUIRED_ISPL = table.Column<int>(type: "integer", nullable: false),
                    ISPLISSYNC = table.Column<int>(type: "integer", nullable: false),
                    ISPLISUPDATED = table.Column<int>(type: "integer", nullable: false),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    OBJECTLIFECYCLEMODELID = table.Column<string>(type: "text", nullable: false),
                    SHORTNAME_ISPL = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicsEquipmentCategory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DynamicsEquipmentMaster",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataAreaId = table.Column<string>(type: "text", nullable: false),
                    OBJECTID = table.Column<string>(type: "text", nullable: false),
                    ACQUISITIONCOST = table.Column<decimal>(type: "numeric", nullable: true),
                    ACQUISITIONDATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ACTIVEFROM = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ACTIVETO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CATEGORYLEVEL1 = table.Column<string>(type: "text", nullable: true),
                    CATEGORYLEVEL2 = table.Column<string>(type: "text", nullable: true),
                    CATEGORYLEVEL3 = table.Column<string>(type: "text", nullable: true),
                    CRITICALITY = table.Column<int>(type: "integer", nullable: true),
                    FIXEDASSETID = table.Column<string>(type: "text", nullable: true),
                    FIXEDASSETNAME = table.Column<string>(type: "text", nullable: true),
                    FUNCTIONALLOCATION = table.Column<string>(type: "text", nullable: true),
                    ISPLBATCHNUMBER = table.Column<string>(type: "text", nullable: true),
                    ISPLHIREDDATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ISPLISHIREDASSET = table.Column<int>(type: "integer", nullable: false),
                    OBJECTACTIVE = table.Column<int>(type: "integer", nullable: false),
                    ISPLISSYNC = table.Column<int>(type: "integer", nullable: false),
                    ISPLISUPDATED = table.Column<int>(type: "integer", nullable: false),
                    ISPLITEMID = table.Column<string>(type: "text", nullable: true),
                    ISPLITEMNAME = table.Column<string>(type: "text", nullable: true),
                    ISPLPONUMBER = table.Column<string>(type: "text", nullable: true),
                    ISPLVEHICLENUMBER = table.Column<string>(type: "text", nullable: true),
                    ISPLVENDORACCOUNT = table.Column<string>(type: "text", nullable: true),
                    MANUFACTURER = table.Column<string>(type: "text", nullable: true),
                    MODEL = table.Column<string>(type: "text", nullable: true),
                    MODELRECID = table.Column<long>(type: "bigint", nullable: true),
                    MODELYEAR = table.Column<string>(type: "text", nullable: true),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    OBJECTIDPARENT = table.Column<string>(type: "text", nullable: true),
                    OBJECTIDPARENTNAME = table.Column<string>(type: "text", nullable: true),
                    OBJECTLIFECYCLESTATE = table.Column<string>(type: "text", nullable: true),
                    OBJECTTYPE = table.Column<string>(type: "text", nullable: true),
                    PURCHID = table.Column<string>(type: "text", nullable: true),
                    RESOURCE = table.Column<string>(type: "text", nullable: true),
                    SERIALID = table.Column<string>(type: "text", nullable: true),
                    SERVICELEVEL = table.Column<int>(type: "integer", nullable: true),
                    STDFUELCONSUMPTION_ISPL = table.Column<float>(type: "real", nullable: true),
                    VENDACCOUNT = table.Column<string>(type: "text", nullable: true),
                    VENDNAME = table.Column<string>(type: "text", nullable: true),
                    WARRANTYDATEFROMVEND = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WARRANTYDATETOVEND = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WARRANTYVEND = table.Column<string>(type: "text", nullable: true),
                    WRKCTRLDESCRIPTION = table.Column<string>(type: "text", nullable: true),
                    HimappProjectID = table.Column<int>(type: "integer", nullable: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicsEquipmentMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentID = table.Column<int>(type: "integer", nullable: true),
                    MaintenancePointID = table.Column<int>(type: "integer", nullable: true),
                    ItemID = table.Column<int>(type: "integer", nullable: true),
                    ReadingTypeID = table.Column<int>(type: "integer", nullable: true),
                    IntervalValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TolerenceValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Months = table.Column<int>(type: "integer", nullable: true),
                    Remarks = table.Column<string>(type: "text", unicode: false, nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentPreventiveMaintenancePoints", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeterReadingType",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReadingTypeName = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", unicode: false, nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadingType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PreventiveMaintenancePoint",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaintenancePointName = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventiveMaintenancePoint", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkDescriptions",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkDescriptionName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDescriptions", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPreventiveMaintenancePoints_EquipmentID",
                schema: "PAndM",
                table: "EquipmentPreventiveMaintenancePoints",
                column: "EquipmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTypes",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "DynamicsEquipmentCategory",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "DynamicsEquipmentMaster",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "MeterReadingType",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "PreventiveMaintenancePoint",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "WorkDescriptions",
                schema: "PAndM");
        }
    }
}
