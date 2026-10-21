using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class Soumy_AddWorkDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PM");

            migrationBuilder.CreateTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PM",
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
                schema: "PM",
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
                schema: "PM",
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
                schema: "pm",
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
                schema: "PM",
                table: "EquipmentPreventiveMaintenancePoints",
                column: "EquipmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentPreventiveMaintenancePoints",
                schema: "PM");

            migrationBuilder.DropTable(
                name: "MeterReadingType",
                schema: "PM");

            migrationBuilder.DropTable(
                name: "PreventiveMaintenancePoint",
                schema: "PM");

            migrationBuilder.DropTable(
                name: "WorkDescriptions",
                schema: "pm");
        }
    }
}
