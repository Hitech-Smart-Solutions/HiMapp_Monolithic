using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_RenameColumnsAndAddColumnInDDLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlipNumber",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                newName: "IssueNumber");

            migrationBuilder.RenameColumn(
                name: "ActivityCategoryID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                newName: "ActivityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssueNumber",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                newName: "SlipNumber");

            migrationBuilder.RenameColumn(
                name: "ActivityID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                newName: "ActivityCategoryID");
        }
    }
}
