using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class AddEquipmentCategoryLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentCategoryBreakdownReasons",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentBreakdownReasonId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategoryBreakdownReasons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryBreakdownReasons_DynamicsEquipmentCategory~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryBreakdownReasons_EquipmentBreakdownReasons~",
                        column: x => x.EquipmentBreakdownReasonId,
                        principalSchema: "PAndM",
                        principalTable: "EquipmentBreakdownReasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategoryChecklistPoints",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentChecklistPointId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategoryChecklistPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryChecklistPoints_DynamicsEquipmentCategory_~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryChecklistPoints_EquipmentChecklistPoints_E~",
                        column: x => x.EquipmentChecklistPointId,
                        principalSchema: "PAndM",
                        principalTable: "EquipmentChecklistPoints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategoryPMPoints",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreventiveMaintenancePointId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategoryPMPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryPMPoints_DynamicsEquipmentCategory_Equipme~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryPMPoints_PreventiveMaintenancePoint_Preven~",
                        column: x => x.PreventiveMaintenancePointId,
                        principalSchema: "PAndM",
                        principalTable: "PreventiveMaintenancePoint",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategorySpecifications",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpecificationMasterId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategorySpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategorySpecifications_DynamicsEquipmentCategory_E~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategorySpecifications_SpecificationMaster_Specifi~",
                        column: x => x.SpecificationMasterId,
                        principalSchema: "PAndM",
                        principalTable: "SpecificationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategorySubParts",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentSubPartId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategorySubParts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategorySubParts_DynamicsEquipmentCategory_Equipme~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategorySubParts_EquipmentSubParts_EquipmentSubPar~",
                        column: x => x.EquipmentSubPartId,
                        principalSchema: "PAndM",
                        principalTable: "EquipmentSubParts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategoryWorkDescriptions",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkDescriptionId = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    EquipmentCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategoryWorkDescriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryWorkDescriptions_DynamicsEquipmentCategory~",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "PAndM",
                        principalTable: "DynamicsEquipmentCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCategoryWorkDescriptions_WorkDescriptions_WorkDesc~",
                        column: x => x.WorkDescriptionId,
                        principalSchema: "PAndM",
                        principalTable: "WorkDescriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryBreakdownReasons_EquipmentBreakdownReasonId",
                schema: "PAndM",
                table: "EquipmentCategoryBreakdownReasons",
                column: "EquipmentBreakdownReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryBreakdownReasons_EquipmentCategoryId_Equip~",
                schema: "PAndM",
                table: "EquipmentCategoryBreakdownReasons",
                columns: new[] { "EquipmentCategoryId", "EquipmentBreakdownReasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryChecklistPoints_EquipmentCategoryId_Equipm~",
                schema: "PAndM",
                table: "EquipmentCategoryChecklistPoints",
                columns: new[] { "EquipmentCategoryId", "EquipmentChecklistPointId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryChecklistPoints_EquipmentChecklistPointId",
                schema: "PAndM",
                table: "EquipmentCategoryChecklistPoints",
                column: "EquipmentChecklistPointId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryPMPoints_EquipmentCategoryId_PreventiveMai~",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                columns: new[] { "EquipmentCategoryId", "PreventiveMaintenancePointId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryPMPoints_PreventiveMaintenancePointId",
                schema: "PAndM",
                table: "EquipmentCategoryPMPoints",
                column: "PreventiveMaintenancePointId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategorySpecifications_EquipmentCategoryId_Specifi~",
                schema: "PAndM",
                table: "EquipmentCategorySpecifications",
                columns: new[] { "EquipmentCategoryId", "SpecificationMasterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategorySpecifications_SpecificationMasterId",
                schema: "PAndM",
                table: "EquipmentCategorySpecifications",
                column: "SpecificationMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategorySubParts_EquipmentCategoryId_EquipmentSubP~",
                schema: "PAndM",
                table: "EquipmentCategorySubParts",
                columns: new[] { "EquipmentCategoryId", "EquipmentSubPartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategorySubParts_EquipmentSubPartId",
                schema: "PAndM",
                table: "EquipmentCategorySubParts",
                column: "EquipmentSubPartId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryWorkDescriptions_EquipmentCategoryId_WorkD~",
                schema: "PAndM",
                table: "EquipmentCategoryWorkDescriptions",
                columns: new[] { "EquipmentCategoryId", "WorkDescriptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategoryWorkDescriptions_WorkDescriptionId",
                schema: "PAndM",
                table: "EquipmentCategoryWorkDescriptions",
                column: "WorkDescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentCategoryBreakdownReasons",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentCategoryChecklistPoints",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentCategoryPMPoints",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentCategorySpecifications",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentCategorySubParts",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentCategoryWorkDescriptions",
                schema: "PAndM");
        }
    }
}
