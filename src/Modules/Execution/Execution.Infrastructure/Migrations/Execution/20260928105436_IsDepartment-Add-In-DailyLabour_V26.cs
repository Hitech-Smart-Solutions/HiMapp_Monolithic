using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class IsDepartmentAddInDailyLabour_V26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDepartment",
                schema: "execution",
                table: "DailyLaborDetails",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDepartment",
                schema: "execution",
                table: "DailyLaborDetails");
        }
    }
}
