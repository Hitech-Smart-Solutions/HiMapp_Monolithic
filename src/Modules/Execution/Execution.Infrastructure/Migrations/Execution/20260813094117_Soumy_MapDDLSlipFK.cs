using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class Soumy_MapDDLSlipFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropColumn(
                name: "DailyDepartmentalLabourSlipID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DDLSlipID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DDLSlipID");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DDLSlipID",
                principalSchema: "execution",
                principalTable: "DailyDepartmentalLabourSlips",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.DropIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DDLSlipID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails");

            migrationBuilder.AddColumn<int>(
                name: "DailyDepartmentalLabourSlipID",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DailyDepartmentalLabourSlipID");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyDepartmentalLabourSlipDetails_DailyDepartmentalLabourS~",
                schema: "execution",
                table: "DailyDepartmentalLabourSlipDetails",
                column: "DailyDepartmentalLabourSlipID",
                principalSchema: "execution",
                principalTable: "DailyDepartmentalLabourSlips",
                principalColumn: "ID");
        }
    }
}
