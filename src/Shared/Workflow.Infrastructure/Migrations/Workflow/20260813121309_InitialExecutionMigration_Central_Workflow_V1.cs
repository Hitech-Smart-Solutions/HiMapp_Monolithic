using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Himapp.Workflow.Infrastructure.Migrations.Workflow
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration_Central_Workflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalWorkflowRoleDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflow",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "CentralApprovalWorkflow",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ApprovalWorkflowCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApprovalWorkflowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    CompanyID = table.Column<int>(type: "integer", nullable: true),
                    LocationID = table.Column<int>(type: "integer", nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentralApprovalWorkflow", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CentralUserRoleMapping",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    RoleCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentralUserRoleMapping", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CentralApprovalWorkflowProjectDetails",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ApprovalWorkflowID = table.Column<int>(type: "integer", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentralApprovalWorkflowProjectDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CentralApprovalWorkflowProjectDetails_CentralApprovalWorkfl~",
                        column: x => x.ApprovalWorkflowID,
                        principalSchema: "public",
                        principalTable: "CentralApprovalWorkflow",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CentralApprovalWorkflowRoleDetails",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ApprovalWorkflowID = table.Column<int>(type: "integer", nullable: false),
                    RoleID = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CanAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    CanUnAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentralApprovalWorkflowRoleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CentralApprovalWorkflowRoleDetails_CentralApprovalWorkflow_~",
                        column: x => x.ApprovalWorkflowID,
                        principalSchema: "public",
                        principalTable: "CentralApprovalWorkflow",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CentralUserRoleMappingDetails",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CentralRoleMappingID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: false),
                    StatusID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentralUserRoleMappingDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CentralUserRoleMappingDetails_CentralUserRoleMapping_Centra~",
                        column: x => x.CentralRoleMappingID,
                        principalSchema: "public",
                        principalTable: "CentralUserRoleMapping",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CentralApprovalWorkflowProjectDetails_ApprovalWorkflowID",
                schema: "public",
                table: "CentralApprovalWorkflowProjectDetails",
                column: "ApprovalWorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_CentralApprovalWorkflowRoleDetails_ApprovalWorkflowID",
                schema: "public",
                table: "CentralApprovalWorkflowRoleDetails",
                column: "ApprovalWorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_CentralUserRoleMappingDetails_CentralRoleMappingID",
                schema: "public",
                table: "CentralUserRoleMappingDetails",
                column: "CentralRoleMappingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CentralApprovalWorkflowProjectDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CentralApprovalWorkflowRoleDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CentralUserRoleMappingDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CentralApprovalWorkflow",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CentralUserRoleMapping",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflow",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApprovalWorkflowCode = table.Column<string>(type: "text", nullable: false),
                    ApprovalWorkflowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyID = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinancialYearID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LocationID = table.Column<int>(type: "integer", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    ProjectID = table.Column<int>(type: "integer", nullable: true),
                    StateID = table.Column<byte>(type: "smallint", nullable: true),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false)
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
                    ApprovalWorkflowID = table.Column<int>(type: "integer", nullable: false),
                    CanAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    CanUnAuthorize = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    RoleID = table.Column<int>(type: "integer", nullable: false),
                    UniqueID = table.Column<Guid>(type: "uuid", nullable: false)
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
    }
}
