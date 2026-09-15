using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class AddIsDepartmentColumnInManpower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDepartment",
                schema: "execution",
                table: "ManpowerDetails",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DLRCode",
                schema: "execution",
                table: "DailyLabor",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyLabor_DLRCode",
                schema: "execution",
                table: "DailyLabor",
                column: "DLRCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyLabor_DLRCode",
                schema: "execution",
                table: "DailyLabor");

            migrationBuilder.DropColumn(
                name: "IsDepartment",
                schema: "execution",
                table: "ManpowerDetails");

            migrationBuilder.AlterColumn<string>(
                name: "DLRCode",
                schema: "execution",
                table: "DailyLabor",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
