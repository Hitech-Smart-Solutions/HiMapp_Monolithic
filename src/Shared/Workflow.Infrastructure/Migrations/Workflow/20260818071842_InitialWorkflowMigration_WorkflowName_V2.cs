using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Workflow.Infrastructure.Migrations.Workflow
{
    /// <inheritdoc />
    public partial class InitialWorkflowMigration_WorkflowName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalWorkflowName",
                schema: "public",
                table: "CentralApprovalWorkflow",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalWorkflowName",
                schema: "public",
                table: "CentralApprovalWorkflow");
        }
    }
}
