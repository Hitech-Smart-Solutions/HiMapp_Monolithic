using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Workflow.Infrastructure.Migrations.Workflow
{
    /// <inheritdoc />
    public partial class InitialWorkflowMigration_WorkflowTypeID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LocationID",
                schema: "public",
                table: "CentralApprovalWorkflow",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowTypeID",
                schema: "public",
                table: "CentralApprovalWorkflow",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowTypeID",
                schema: "public",
                table: "CentralApprovalWorkflow");

            migrationBuilder.AlterColumn<int>(
                name: "LocationID",
                schema: "public",
                table: "CentralApprovalWorkflow",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
