using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_RemoveCompanyIDLocationIDAndFinancialYearID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");

            migrationBuilder.DropColumn(
                name: "FinancialYearID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");

            migrationBuilder.DropColumn(
                name: "LocationID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialYearID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "integer",
                nullable: true);
        }
    }
}
