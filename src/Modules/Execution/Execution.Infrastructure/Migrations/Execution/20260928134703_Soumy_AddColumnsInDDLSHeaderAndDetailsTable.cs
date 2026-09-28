using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_AddColumnsInDDLSHeaderAndDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Mandays",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalHours",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                schema: "execution",
                table: "DailyDepartmentalLabourSlips");

            migrationBuilder.DropColumn(
                name: "Mandays",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropColumn(
                name: "Rate",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropColumn(
                name: "TotalHours",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");
        }
    }
}
