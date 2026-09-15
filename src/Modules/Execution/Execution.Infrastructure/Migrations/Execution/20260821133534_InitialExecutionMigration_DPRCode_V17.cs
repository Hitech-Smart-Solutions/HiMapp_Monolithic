using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.Execution.Infrastructure.Migrations.Execution
{
    /// <inheritdoc />
    public partial class InitialExecutionMigration_DPRCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileSize",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DPRCode",
                schema: "execution",
                table: "DailyProgress",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropColumn(
                name: "FileType",
                schema: "execution",
                table: "DailyProgressPhotos");

            migrationBuilder.DropColumn(
                name: "DPRCode",
                schema: "execution",
                table: "DailyProgress");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                schema: "execution",
                table: "DailyProgressPhotos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
