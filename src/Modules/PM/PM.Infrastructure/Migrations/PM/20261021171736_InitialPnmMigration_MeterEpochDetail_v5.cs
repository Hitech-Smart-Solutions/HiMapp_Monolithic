using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <inheritdoc />
    public partial class InitialPnmMigration_MeterEpochDetail_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentMeterCumulativeDetail",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentID = table.Column<int>(type: "integer", nullable: true),
                    FinalReadingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ActualOpeningReadingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMeterCumulativeDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMeterEpochDetails",
                schema: "PAndM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentID = table.Column<int>(type: "integer", nullable: true),
                    OpenningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MeterReadingID = table.Column<int>(type: "integer", nullable: true),
                    ResetReadingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    OpeningReadingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ReasonID = table.Column<int>(type: "integer", nullable: true),
                    Remarks = table.Column<string>(type: "text", unicode: false, nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMeterEpochDetails", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMeterCumulativeDetail_EquipmentID",
                schema: "PAndM",
                table: "EquipmentMeterCumulativeDetail",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMeterEpochDetails_EquipmentID",
                schema: "PAndM",
                table: "EquipmentMeterEpochDetails",
                column: "EquipmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentMeterCumulativeDetail",
                schema: "PAndM");

            migrationBuilder.DropTable(
                name: "EquipmentMeterEpochDetails",
                schema: "PAndM");
        }
    }
}
