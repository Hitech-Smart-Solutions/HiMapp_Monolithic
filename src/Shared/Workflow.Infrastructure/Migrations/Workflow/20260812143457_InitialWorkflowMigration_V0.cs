using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Workflow.Infrastructure.Migrations.Workflow
{
    /// <inheritdoc />
    public partial class InitialWorkflowMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflow",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalWorkflowCode = table.Column<string>(type: "text", nullable: false),
                    ApprovalWorkflowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    CompanyID = table.Column<int>(type: "integer", nullable: true),
                    LocationID = table.Column<int>(type: "integer", nullable: true),
                    ProjectID = table.Column<int>(type: "integer", nullable: true),
                    FinancialYearID = table.Column<int>(type: "integer", nullable: true),
                    StateID = table.Column<byte>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflow", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflowRoleDetails",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalWorkflowID = table.Column<int>(type: "integer", nullable: false),
                    RoleID = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    CanAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    CanUnAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflowRoleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflowRoleDetails_ApprovalWorkflow_ApprovalWorkfl~",
                        column: x => x.ApprovalWorkflowID,
                        principalSchema: "public",
                        principalTable: "ApprovalWorkflow",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflowRoleDetails_ApprovalWorkflowID",
                schema: "public",
                table: "ApprovalWorkflowRoleDetails",
                column: "ApprovalWorkflowID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalWorkflowRoleDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflow",
                schema: "public");
        }
    }
}
