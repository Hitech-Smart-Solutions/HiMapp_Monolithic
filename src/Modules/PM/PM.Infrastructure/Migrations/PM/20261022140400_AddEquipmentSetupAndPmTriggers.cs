using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class AddEquipmentSetupAndPmTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "IntervalValue",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Months",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthsTolerance",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                type: "numeric(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToleranceValue",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                type: "numeric(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsSyncLog",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsEquipmentMaster",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsEquipmentCategory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EquipmentSetupDocuments",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StoredFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OriginalFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSetupDocuments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "PAndM",
                        principalTable: "DocumentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupDocuments_DynamicsEquipmentMaster_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSetupPMPoints",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    PreventiveMaintenancePointId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false),
                    ReadingTypeId = table.Column<int>(type: "integer", nullable: true),
                    IntervalValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    ToleranceValue = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    Months = table.Column<int>(type: "integer", nullable: true),
                    MonthsTolerance = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSetupPMPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupPMPoints_DynamicsEquipmentMaster_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupPMPoints_MeterReadingType_ReadingTypeId",
                        column: x => x.ReadingTypeId,
                        principalSchema: "PAndM",
                        principalTable: "MeterReadingType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupPMPoints_PreventiveMaintenancePoint_Preventiv~",
                        column: x => x.PreventiveMaintenancePointId,
                        principalSchema: "PAndM",
                        principalTable: "PreventiveMaintenancePoint",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSetups",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSetups_DynamicsEquipmentMaster_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSetupSpecifications",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    SpecificationMasterId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSetupSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupSpecifications_DynamicsEquipmentMaster_Equipm~",
                        column: x => x.EquipmentId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupSpecifications_SpecificationMaster_Specificat~",
                        column: x => x.SpecificationMasterId,
                        principalSchema: "PAndM",
                        principalTable: "SpecificationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSetupSteps",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    StepNo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSetupSteps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSetupSteps_DynamicsEquipmentMaster_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryPMPoints_ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                column: "ReadingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupDocuments_DocumentTypeId",
                schema: "PAndM",
                table: "EquipmentSetupDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupDocuments_EquipmentId",
                schema: "PAndM",
                table: "EquipmentSetupDocuments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupPMPoints_EquipmentId_PreventiveMaintenancePoi~",
                schema: "PAndM",
                table: "EquipmentSetupPMPoints",
                columns: new[] { "EquipmentId", "PreventiveMaintenancePointId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupPMPoints_PreventiveMaintenancePointId",
                schema: "PAndM",
                table: "EquipmentSetupPMPoints",
                column: "PreventiveMaintenancePointId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupPMPoints_ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentSetupPMPoints",
                column: "ReadingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetups_EquipmentId",
                schema: "PAndM",
                table: "EquipmentSetups",
                column: "EquipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupSpecifications_EquipmentId_SpecificationMaste~",
                schema: "PAndM",
                table: "EquipmentSetupSpecifications",
                columns: new[] { "EquipmentId", "SpecificationMasterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupSpecifications_SpecificationMasterId",
                schema: "PAndM",
                table: "EquipmentSetupSpecifications",
                column: "SpecificationMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSetupSteps_EquipmentId_StepNo",
                schema: "PAndM",
                table: "EquipmentSetupSteps",
                columns: new[] { "EquipmentId", "StepNo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentCategoryPMPoints_MeterReadingType_ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                column: "ReadingTypeId",
                principalSchema: "PAndM",
                principalTable: "MeterReadingType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentCategoryPMPoints_MeterReadingType_ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropTable(
                name: "EquipmentSetupDocuments",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentSetupPMPoints",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentSetups",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentSetupSpecifications",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentSetupSteps",
                schema: "PAndM");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentCategoryPMPoints_ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "IntervalValue",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "Months",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "MonthsTolerance",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "ReadingTypeId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "ToleranceValue",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsSyncLog");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsEquipmentMaster");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "PAndM",
                table: "DynamicsEquipmentCategory");
        }
    }
}
