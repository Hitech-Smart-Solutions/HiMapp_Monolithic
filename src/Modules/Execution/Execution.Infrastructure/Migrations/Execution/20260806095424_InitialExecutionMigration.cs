using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanType",
                schema: "execution",
                table: "Plannings");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "execution",
                table: "Plannings");

            migrationBuilder.AddColumn<int>(
                name: "PlanTypeID",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                schema: "execution",
                table: "Plannings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlanningDocumentDetail",
                schema: "execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanningID = table.Column<int>(type: "integer", nullable: false),
                    DocumentName = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningDocumentDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlanningDocumentDetail_Plannings_PlanningID",
                        column: x => x.PlanningID,
                        principalSchema: "execution",
                        principalTable: "Plannings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanningDocumentDetail_PlanningID",
                schema: "execution",
                table: "PlanningDocumentDetail",
                column: "PlanningID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanningDocumentDetail",
                schema: "execution");

            migrationBuilder.DropColumn(
                name: "PlanTypeID",
                schema: "execution",
                table: "Plannings");

            migrationBuilder.DropColumn(
                name: "StatusID",
                schema: "execution",
                table: "Plannings");

            migrationBuilder.AddColumn<string>(
                name: "PlanType",
                schema: "execution",
                table: "Plannings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "execution",
                table: "Plannings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
