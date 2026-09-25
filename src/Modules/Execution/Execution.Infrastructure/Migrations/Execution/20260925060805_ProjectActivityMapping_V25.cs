using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class ProjectActivityMapping_V25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DDLSBackDatedDays",
                schema: "execution",
                table: "ExecutionProjectConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DDLSDPRLinkage",
                schema: "execution",
                table: "ExecutionProjectConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DPREntryContinue",
                schema: "execution",
                table: "ExecutionProjectConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlanningRequired",
                schema: "execution",
                table: "ExecutionProjectConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DDLSBackDatedDays",
                schema: "execution",
                table: "ExecutionProjectConfigs");

            migrationBuilder.DropColumn(
                name: "DDLSDPRLinkage",
                schema: "execution",
                table: "ExecutionProjectConfigs");

            migrationBuilder.DropColumn(
                name: "DPREntryContinue",
                schema: "execution",
                table: "ExecutionProjectConfigs");

            migrationBuilder.DropColumn(
                name: "PlanningRequired",
                schema: "execution",
                table: "ExecutionProjectConfigs");
        }
    }
}
